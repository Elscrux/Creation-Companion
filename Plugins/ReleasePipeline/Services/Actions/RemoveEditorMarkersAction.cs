using CreationEditor;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using NiflySharp;
using NiflySharp.Blocks;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using Serilog;
namespace ReleasePipeline.Services.Actions;

public sealed class RemoveEditorMarkersConfig : IConfigDefinition {
    public static TextConfigOption MarkerNameOption { get; } = new(
        "Marker Name",
        "Block name that identifies editor marker blocks. All blocks with this name are removed",
        "EditorMarker"
    );

    public static BoolConfigOption RemoveUnreferencedBlocksOption { get; } = new(
        "Remove Unreferenced Blocks",
        "If true, blocks that are no longer referenced after removing the editor markers (e.g. their children) are removed as well",
        true
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        MarkerNameOption,
        RemoveUnreferencedBlocksOption,
    ];
}

/// <summary>
/// Pipeline action that removes editor marker blocks from all NIFs in the release folder.
/// PlayStation builds cannot hide editor markers at runtime, so they have to be removed from the meshes.
/// Should run after the NIFs have been copied into the release folder.
/// </summary>
public sealed class RemoveEditorMarkersAction(ILogger logger) : IPipelineAction {
    public Guid Id { get; } = new("7c1e2f4a-9b3d-4e68-a5c7-2d8f0b6e9a41");
    public string DisplayName => "Remove Editor Markers";
    public string Description =>
        "Removes editor marker blocks from all NIFs in the release folder. Required for PlayStation builds, which cannot hide editor markers at runtime.";
    public bool RequiresInPlaceModification => true;
    public IConfigDefinition ConfigDefinition { get; } = new RemoveEditorMarkersConfig();
    public ConfigValues ConfigValues { get; set; } = new();

    public Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token) {
        var markerNamePrefix = ConfigValues.Get(RemoveEditorMarkersConfig.MarkerNameOption);
        var removeUnreferencedBlocks = ConfigValues.Get(RemoveEditorMarkersConfig.RemoveUnreferencedBlocksOption);

        var modifiedFiles = 0;
        var removedMarkerBlocks = 0;
        var removedUnreferencedBlocks = 0;
        var failedFiles = 0;

        var nifFiles = context.FileSystem.Directory.GetFiles(context.WorkingDirectory, "*.nif", SearchOption.AllDirectories);
        Parallel.ForEach(nifFiles,
            new ParallelOptions { CancellationToken = token },
            (file, state) => {
                if (state.ShouldExitCurrentIteration) return;

                var relativePath = context.FileSystem.Path.GetRelativePath(context.ReleaseFolder, file);

                try {
                    var nif = new NifFile();
                    if (nif.Load(file) != 0) {
                        logger.Here().Warning("Couldn't load nif {File}, skipping", file);
                        failedFiles++;
                        return;
                    }

                    var markerBlocks = nif.Blocks
                        .OfType<NiObjectNET>()
                        .Where(block => block.Name?.String?.Equals(markerNamePrefix, StringComparison.OrdinalIgnoreCase) ?? false)
                        .ToList();

                    if (markerBlocks.Count == 0) return;

                    foreach (var markerBlock in markerBlocks) {
                        if (!nif.GetBlockIndex(markerBlock, out var blockId)) continue;
                        if (!nif.RemoveBlock(blockId)) continue;

                        removedMarkerBlocks++;
                        logger.Here().Information("Removed editor marker block {BlockName} from {File}", markerBlock.Name?.String, relativePath);
                    }

                    if (removeUnreferencedBlocks) {
                        removedUnreferencedBlocks += nif.RemoveUnreferencedBlocks();
                    }

                    if (nif.Save(file) != 0) {
                        logger.Here().Error("Couldn't save modified nif {File}", relativePath);
                        failedFiles++;
                        return;
                    }

                    modifiedFiles++;
                } catch (Exception e) {
                    logger.Here().Error(e, "Error removing editor markers from nif {File}: {Exception}", relativePath, e.Message);
                    failedFiles++;
                }
            });

        var message = removedMarkerBlocks == 0
            ? $"No editor marker blocks found in {nifFiles.Length} NIFs."
            : $"Removed {removedMarkerBlocks} editor marker blocks from {modifiedFiles} of {nifFiles.Length} NIFs.";
        if (removedUnreferencedBlocks > 0) {
            message += $" Also removed {removedUnreferencedBlocks} unreferenced blocks.";
        }
        if (failedFiles > 0) {
            message += $" Failed to process {failedFiles} NIFs.";
            return Task.FromResult(PipelineActionResult.Fail(message));
        }

        return Task.FromResult(PipelineActionResult.Pass(message));
    }
}
