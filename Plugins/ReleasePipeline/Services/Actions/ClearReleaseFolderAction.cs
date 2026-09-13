using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Services.Actions;

/// <summary>Configuration for <see cref="ClearReleaseFolderAction"/>.</summary>
public sealed class ClearReleaseFolderConfig {
    /// <summary>When true, the release folder is emptied before packaging.</summary>
    public bool DeleteContents { get; set; } = true;
}

/// <summary>
/// Empties the release folder before packaging so a run always starts clean.
/// Only clears the exact folder named by the pipeline or runner.
/// </summary>
public sealed class ClearReleaseFolderAction : IPipelineAction<ClearReleaseFolderConfig> {
    public Guid Id { get; } = new("b7c2e3a1-4d4a-4f7a-9a4c-1e6a9b200002");
    public string DisplayName => "Clear Release Folder";
    public string Description => "Deletes the current contents of the release folder so packaging starts from an empty directory.";
    public ClearReleaseFolderConfig Config { get; set; } = new();
    public Type ConfigType => typeof(ClearReleaseFolderConfig);

    public Task<PipelineActionResult> ExecuteAsync(PipelineContext context, CancellationToken token) {
        var config = Config;
        var fileSystem = context.FileSystem;

        if (!config.DeleteContents) {
            return Task.FromResult(PipelineActionResult.Pass("Skipped clearing release folder (disabled in config)."));
        }

        if (!fileSystem.Directory.Exists(context.ReleaseFolder)) {
            return Task.FromResult(PipelineActionResult.Pass("Release folder is already empty or missing."));
        }

        foreach (var entry in fileSystem.Directory.GetFileSystemEntries(context.ReleaseFolder)) {
            token.ThrowIfCancellationRequested();
            if (fileSystem.Directory.Exists(entry)) {
                fileSystem.Directory.Delete(entry, true);
            } else {
                fileSystem.File.Delete(entry);
            }
        }

        return Task.FromResult(PipelineActionResult.Pass($"Cleared release folder '{context.ReleaseFolder}'."));
    }
}
