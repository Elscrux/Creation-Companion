using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Services.Actions;

public sealed class ExcludePathActionConfig : IConfigDefinition {
    public static TextConfigOption ExcludedPathsOption { get; } = new(
        "Excluded Paths",
        "Paths to exclude from the release folder, separated by commas. Example: some/folder, some/file.txt",
        string.Empty
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ExcludedPathsOption,
    ];
}

public sealed class ExcludePathAction : IPipelineAction {
    public Guid Id { get; } = new("11587aec-a0ef-4d1c-9887-afeb0adec7ff");
    public string DisplayName => "Exclude Paths";
    public string Description => "Excludes a specific path of a file or folder from the release folder, preventing it from being included in the final release.";
    public bool RequiresInPlaceModification => true;
    public IConfigDefinition ConfigDefinition { get; } = new ExcludePathActionConfig();
    public ConfigValues ConfigValues { get; set; } = new();

    public Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token) {
        var excludedPaths = ConfigValues.Get(ExcludePathActionConfig.ExcludedPathsOption)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var excludedCount = 0;
        foreach (var excludedPath in excludedPaths) {
            var fullPath = context.FileSystem.Path.Combine(context.WorkingDirectory, excludedPath);
            if (context.FileSystem.Directory.Exists(fullPath)) {
                context.FileSystem.Directory.Delete(fullPath, true);
                excludedCount++;
            } else if (context.FileSystem.File.Exists(fullPath)) {
                context.FileSystem.File.Delete(fullPath);
                excludedCount++;
            }
        }

        return Task.FromResult(PipelineActionResult.Pass($"Excluded {excludedCount} paths successfully."));
    }
}
