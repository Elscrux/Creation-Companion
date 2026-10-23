using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Services.Actions;

/// <summary>Configuration for <see cref="ClearReleaseFolderAction"/></summary>
public sealed class ClearReleaseFolderConfig : IConfigDefinition {
    public static BoolConfigOption DeleteContentsOption { get; } = new(
        "Delete Contents",
        "Clear the release folder before packaging, otherwise the folder will be left as-is",
        true
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        DeleteContentsOption,
    ];
}

/// <summary>
/// Empties the release folder before packaging so a run always starts clean.
/// Only clears the exact folder named by the pipeline or runner.
/// </summary>
public sealed class ClearReleaseFolderAction : IPipelineAction {
    public Guid Id { get; } = new("b1848170-35f7-41a3-a887-d81595275265");
    public string DisplayName => "Clear Release Folder";
    public string Description => "Deletes the current contents of the release folder so packaging starts from an empty directory.";
    public bool RequiresInPlaceModification => false;
    public IConfigDefinition ConfigDefinition { get; } = new ClearReleaseFolderConfig();
    public ConfigValues ConfigValues { get; set; } = new();

    public Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token) {
        var deleteContents = ConfigValues.Get(ClearReleaseFolderConfig.DeleteContentsOption);
        if (!deleteContents) {
            return Task.FromResult(PipelineActionResult.Pass("Skipped clearing release folder (disabled in config)."));
        }

        if (!context.FileSystem.Directory.Exists(context.ReleaseFolder)) {
            return Task.FromResult(PipelineActionResult.Pass("Release folder is already empty or missing."));
        }

        foreach (var entry in context.FileSystem.Directory.GetFileSystemEntries(context.ReleaseFolder)) {
            token.ThrowIfCancellationRequested();
            if (context.FileSystem.Directory.Exists(entry)) {
                context.FileSystem.Directory.Delete(entry, true);
            } else {
                context.FileSystem.File.Delete(entry);
            }
        }

        return Task.FromResult(PipelineActionResult.Pass($"Cleared release folder '{context.ReleaseFolder}'."));
    }
}
