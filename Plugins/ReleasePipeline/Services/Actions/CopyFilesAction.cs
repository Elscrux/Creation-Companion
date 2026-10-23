using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using Serilog;
namespace ReleasePipeline.Services.Actions;

public sealed class CopyFilesConfig : IConfigDefinition {
    public static TextConfigOption FileExtensionsOption { get; } = new(
        "File Extensions",
        "File extensions to copy, separated by commas. Example: .esp,.esm,.bsa",
        string.Empty
    );

    public static BoolConfigOption KeepDirectoryStructureOption { get; } = new(
        "Keep Directory Structure",
        "Preserve the original directory structure in the release folder, otherwise all files will be copied directly into the release folder",
        true
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        FileExtensionsOption,
        KeepDirectoryStructureOption,
    ];
}

/// <summary>
/// Pipeline action that copies files with specified extensions from the mod's data source to the release folder.
/// </summary>
public sealed class CopyFilesAction(ILogger logger, IFileSystem fileSystem, IIgnoredDirectoriesProvider ignoredDirectoriesProvider) : IPipelineAction {
    public Guid Id { get; } = new("6d4a8d73-4a05-4d6f-9331-523d1da2b615");
    public string DisplayName => "Copy Files";
    public string Description => "Copies files with specified extensions from the mod's data source to the release folder.";
    public bool RequiresInPlaceModification => false;
    public IConfigDefinition ConfigDefinition { get; } = new CopyFilesConfig();
    public ConfigValues ConfigValues { get; set; } = new();

    public Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token) {
        var fileExtensions = ConfigValues.Get(CopyFilesConfig.FileExtensionsOption)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var keepDirectoryStructure = ConfigValues.Get(CopyFilesConfig.KeepDirectoryStructureOption);

        foreach (var extension in fileExtensions) {
            var filesToCopy = fileSystem.Directory.GetFiles(context.WorkingDirectory, $"*{extension}", SearchOption.AllDirectories);
            foreach (var file in filesToCopy) {
                var relativePath = fileSystem.Path.GetRelativePath(context.WorkingDirectory, file);
                if (ignoredDirectoriesProvider.IsIgnored(relativePath)) continue;

                var pathInReleaseFolder = keepDirectoryStructure
                    ? relativePath
                    : fileSystem.Path.GetFileName(file);

                var destinationPath = fileSystem.Path.Combine(context.ReleaseFolder, pathInReleaseFolder);
                fileSystem.Directory.CreateDirectory(fileSystem.Path.GetDirectoryName(destinationPath)!);
                fileSystem.File.Copy(file, destinationPath, true);
                logger.Here().Information("Copied {File} to {Destination}", file, destinationPath);
            }
        }

        return Task.FromResult(PipelineActionResult.Pass("Files copied successfully."));
    }
}
