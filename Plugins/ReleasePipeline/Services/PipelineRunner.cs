using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Analyzers;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using ReleasePipeline.Models.Check;
using Serilog;
namespace ReleasePipeline.Services;

/// <summary>
/// Runs the pre-release check suite and then executes a pipeline's ordered actions into a
/// separate release folder. A failed action stops the whole run.
/// </summary>
public sealed class PipelineRunner(
    IReadOnlyList<IReleaseCheck> checks,
    IIgnoredDirectoriesProvider ignoredDirectoriesProvider,
    IEditorEnvironment editorEnvironment,
    IFileSystem fileSystem,
    ILogger logger) {

    /// <summary>The fixed, non-configurable check suite</summary>
    public IReadOnlyList<IReleaseCheck> Checks { get; } = checks;

    /// <summary>Runs the fixed check suite against the mod. Checks that don't apply are skipped</summary>
    public async Task<IReadOnlyList<CheckOutcome>> RunChecksAsync(
        ISkyrimModGetter mod,
        CancellationToken token) {

        var relevantChecks = Checks
            .Where(check => check.AppliesTo(mod))
            .ToArray();

        var otherModsInLoadOrder = editorEnvironment.LinkCache.PriorityOrder
            .Select(x => x.ModKey)
            .Where(modKey => modKey != mod.ModKey)
            .ToArray();

        var analyzer = AnalyzerRunnerBuilder.Create(GameRelease.SkyrimSE)
            .WithLinkCache(editorEnvironment.LinkCache)
            .WithAnalyzers(relevantChecks)
            .WithBlacklistedMods(otherModsInLoadOrder)
            .WithFileSystem(fileSystem)
            .Build();

        var resultsPerCheck = new Dictionary<IReleaseCheck, List<AnalyzerResult>>();
        await foreach (var result in analyzer.Analyze().WithCancellation(token)) {
            var owningCheck = relevantChecks.Find(check => check.Topics.Contains(result.Topic.TopicDefinition));
            if (owningCheck is null) {
                logger.Here().Warning(
                    "Analyzer produced a result for topic {Topic} but no check owns that topic",
                    result.Topic);
                continue;
            }

            var results = resultsPerCheck.GetOrAdd(owningCheck);
            results.Add(result);
        }

        return relevantChecks.Select(check => new CheckOutcome(
                check,
                resultsPerCheck.TryGetValue(check, out var results)
                    ? new CheckResult(CheckStatus.Warned, results)
                    : new CheckResult(CheckStatus.Passed, [])))
            .ToArray();
    }

    /// <summary>
    /// Runs the enabled actions of the pipeline in order. Stops on the first failure.
    /// Before the first action that modifies files in place, the whole data source is copied
    /// into a temporary working directory and the context is switched to it, so the data source
    /// itself is never modified. The working directory is always cleaned up afterwards.
    /// </summary>
    /// <param name="pipeline">Pipeline to run.</param>
    /// <param name="pipelineContext">Context to pass to each action.</param>
    /// <param name="token">Cancellation token.</param>
    /// <param name="onlyActionId">
    /// When set, only that single action is run (advanced debug option); otherwise the whole
    /// ordered pipeline is executed.
    /// </param>
    public async Task<PipelineRunOutcome> RunPipelineAsync(
        Pipeline pipeline,
        PipelineContext pipelineContext,
        CancellationToken token,
        Guid? onlyActionId = null,
        IObserver<ActionRunProgress>? progress = null) {

        // Create a dedicated release folder for this pipeline run
        var pipelineFolder = fileSystem.Path.Combine(pipelineContext.ReleaseFolder, pipeline.Name);
        fileSystem.Directory.CreateDirectory(pipelineFolder);

        var context = new PipelineContext(
            pipelineContext.Mod,
            pipelineContext.DataSource,
            pipelineContext.AdditionalDataSources,
            pipelineFolder);

        string? workingDirectory = null;
        try {
            // If there are additional data sources, set up a working directory and copy the main data source and all additional ones into it
            if (pipelineContext.AdditionalDataSources.Count > 0) {
                context.WorkingDirectory = workingDirectory = CreateWorkingDirectoryCopy(context, token);
                foreach (var dataSource in pipelineContext.AdditionalDataSources) {
                    CopyDirectory(dataSource, workingDirectory, token);
                }
            }

            var outcomes = new List<ActionRunOutcome>();
            foreach (var action in pipeline.Actions.Where(ShouldRun).ToArray()) {
                token.ThrowIfCancellationRequested();

                // As soon as the first action wants to modify files in place, create a working directory copy of the data source and switch the context to it
                // This ensures that the original data source is never modified
                if (action.RequiresInPlaceModification && workingDirectory is null) {
                    context.WorkingDirectory = workingDirectory = CreateWorkingDirectoryCopy(context, token);
                }

                progress?.OnNext(new ActionRunProgress(action, RunStatus.Running));

                var result = await action.Execute(context, token);
                outcomes.Add(new ActionRunOutcome(action, result));

                progress?.OnNext(new ActionRunProgress(action, result.Status, result.Message));

                logger.Here().Information(
                    "Pipeline '{PipelineName}' action '{ActionName}' finished with {Status}: {Message}",
                    pipeline.Name,
                    action.DisplayName,
                    result.Status,
                    result.Message);

                if (result.Status != RunStatus.Passed && result.Status != RunStatus.Skipped) {
                    return new PipelineRunOutcome(false, outcomes);
                }
            }

            return new PipelineRunOutcome(true, outcomes);
        } catch (Exception ex) {
            logger.Here().Error(ex, "An error occurred while running the release pipeline '{PipelineName}': {Message}", pipeline.Name, ex.Message);
            return new PipelineRunOutcome(false, []);
        } finally {
            if (workingDirectory is not null && context.FileSystem.Directory.Exists(workingDirectory)) {
                logger.Here().Debug("Deleting release pipeline working directory {WorkingDirectory}", workingDirectory);
                context.FileSystem.Directory.Delete(workingDirectory, true);
            }
        }

        // When isolating a single action (debug), run only that action; otherwise run all in order.
        bool ShouldRun(IPipelineAction action) =>
            onlyActionId is null || onlyActionId == action.Id;
    }

    /// <summary>
    /// Copies the entire data source into a fresh temporary directory that actions may modify freely.
    /// </summary>
    private string CreateWorkingDirectoryCopy(PipelineContext context, CancellationToken token) {
        var workingDirectory = context.FileSystem.Path.Combine(
            context.FileSystem.Path.GetTempPath(),
            "ReleasePipeline",
            context.FileSystem.Path.GetRandomFileName());
        context.FileSystem.Directory.CreateDirectory(workingDirectory);

        logger.Here().Debug(
            "Copying data source {DataSource} to release pipeline working directory {WorkingDirectory} for in-place modifications",
            context.DataSource.Path,
            workingDirectory);
        CopyDirectory(context.DataSource, workingDirectory, token);

        return workingDirectory;
    }

    private void CopyDirectory(IDataSource dataSource, string targetDirectory, CancellationToken token) {
        fileSystem.Directory.CreateDirectory(targetDirectory);
        foreach (var file in dataSource.EnumerateFiles(string.Empty, includeSubDirectories: true)) {
            if (ignoredDirectoriesProvider.IsIgnored(file.DataRelativePath)) continue;

            token.ThrowIfCancellationRequested();
            var targetFilePath = fileSystem.Path.Combine(targetDirectory, file.DataRelativePath.Path);
            var directoryFilePath = fileSystem.Path.GetDirectoryName(targetFilePath);
            if (directoryFilePath is not null) {
                fileSystem.Directory.CreateDirectory(directoryFilePath);
            }
            fileSystem.File.Copy(file.FullPath, targetFilePath, true);
        }
    }
}
