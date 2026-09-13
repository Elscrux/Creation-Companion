using System.IO.Abstractions;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using ReleasePipeline.Models.Check;
using Serilog;
namespace ReleasePipeline.Services;

/// <summary>
/// Runs the pre-release check suite and then executes a pipeline's ordered actions into a
/// separate release folder. A failed action stops the whole run.
/// </summary>
public sealed class PipelineRunner(IReadOnlyList<IReleaseCheck> checks, IFileSystem fileSystem, ILogger logger) {

    /// <summary>The fixed, non-configurable check suite.</summary>
    public IReadOnlyList<IReleaseCheck> Checks { get; } = checks;

    /// <summary>Runs the fixed check suite against the mod. Checks that don't apply are skipped.</summary>
    public async Task<IReadOnlyList<CheckOutcome>> RunChecksAsync(
        ISkyrimModGetter mod,
        CancellationToken token) {
        var outcomes = new List<CheckOutcome>();
        foreach (var check in checks) {
            token.ThrowIfCancellationRequested();

            if (!check.AppliesTo(mod)) {
                outcomes.Add(new CheckOutcome(check, CheckResult.Pass()));
                continue;
            }

            var result = await check.RunAsync(mod, token);
            outcomes.Add(new CheckOutcome(check, result));
        }

        return outcomes;
    }

    /// <summary>
    /// Runs the enabled actions of the pipeline in order. Stops on the first failure.
    /// </summary>
    /// <param name="pipeline">Pipeline to run.</param>
    /// <param name="mod">Mod to release.</param>
    /// <param name="defaultReleaseFolder">Fallback folder if the pipeline has none configured.</param>
    /// <param name="token">Cancellation token.</param>
    /// <param name="onlyActionId">
    /// When set, only that single action is run (advanced debug option); otherwise the whole
    /// ordered pipeline is executed.
    /// </param>
    public async Task<PipelineRunOutcome> RunPipelineAsync(
        Pipeline pipeline,
        ISkyrimModGetter mod,
        string defaultReleaseFolder,
        CancellationToken token,
        Guid? onlyActionId = null,
        IObserver<ActionRunProgress>? progress = null) {
        var releaseFolder = string.IsNullOrWhiteSpace(pipeline.ReleaseFolder)
            ? defaultReleaseFolder
            : pipeline.ReleaseFolder;

        fileSystem.Directory.CreateDirectory(releaseFolder);

        var context = new PipelineContext(mod, releaseFolder, fileSystem);

        var outcomes = new List<ActionRunOutcome>();
        foreach (var action in pipeline.Actions.Where(ShouldRun)) {
            token.ThrowIfCancellationRequested();

            progress?.OnNext(new ActionRunProgress(action, RunStatus.Running));

            var result = await action.ExecuteAsync(context, token);
            outcomes.Add(new ActionRunOutcome(action, result));

            progress?.OnNext(new ActionRunProgress(action, result.Status, result.Message));

            logger.Information(
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

        // When isolating a single action (debug), run only that action; otherwise run all in order.
        bool ShouldRun(IPipelineAction action) =>
            onlyActionId is null || onlyActionId == action.Id;
    }
}
