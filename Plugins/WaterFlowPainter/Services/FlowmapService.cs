using Avalonia.Platform;
using CreationEditor;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Integrations.Textures;
using CreationEditor.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Serilog;
using SkiaSharp;
using WaterFlowPainter.Model;
namespace WaterFlowPainter.Services;

/// <summary>
/// Provides access to the water flowmaps of a worldspace. Flowmaps are stored per cell as
/// <c>Textures\Water\&lt;ModFileName&gt;\Flow.{x}.{y}.dds</c>, matching the convention of the water gradient generator.
/// </summary>
public sealed class FlowmapService(
    ILogger logger,
    DdsDecoder ddsDecoder,
    IDataSourceService dataSourceService,
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment) {
    private static readonly Uri FlowTemplateUri = new("avares://WaterFlowPainter/Assets/FlowTemplate.dds");

    public sealed record FlowmapCell(P2Int Position, float WaterHeight, string? FlowmapPath, ICellGetter Cell);

    /// <summary>
    /// Enumerates all exterior cells of the worldspace that have water with the flowmap flag enabled.
    /// </summary>
    public List<FlowmapCell> GetFlowmapCells(FormKey worldspaceFormKey) {
        var cells = new List<FlowmapCell>();
        foreach (var cell in editorEnvironment.LinkCache.EnumerateAllCells(worldspaceFormKey)) {
            if (cell.Grid is not {} grid) continue;
            if (cell.Water.IsNull) continue;
            if (cell.Water.TryResolve(editorEnvironment.LinkCache) is not {} water) continue;
            if ((water.Flags & Water.Flag.EnableFlowmap) == 0) continue;

            var flowmapPath = FindFlowmapPath(grid.Point);
            cells.Add(new FlowmapCell(grid.Point, cell.WaterHeight ?? 0, flowmapPath, cell));
        }

        return cells;
    }

    /// <summary>
    /// Finds the flowmap file of a cell in the data sources.
    /// </summary>
    public string? FindFlowmapPath(P2Int position) {
        var fileName = $"Flow.{position.X}.{position.Y}.dds";

        var activeModPath = Path.Combine("Textures", "Water", editorEnvironment.ActiveMod.ModKey.FileName, fileName);
        if (dataSourceService.GetFileLink(activeModPath) is {} activeLink) return activeLink.FullPath;

        return null;
    }

    /// <summary>Loads the flowmap of a cell. Missing or unreadable flowmaps fall back to a blank flowmap.</summary>
    public FlowmapData LoadFlowmap(string? flowmapPath) {
        if (flowmapPath is null || !File.Exists(flowmapPath)) return CreateBlankFlowmap();

        var bitmap = ddsDecoder.Decode(flowmapPath);
        if (bitmap is null) {
            logger.Here().Warning("Failed to decode flowmap {Path}, using blank flowmap instead", flowmapPath);
            return CreateBlankFlowmap();
        }

        if (bitmap.Width != FlowmapData.Size || bitmap.Height != FlowmapData.Size) {
            logger.Here().Warning("Flowmap {Path} has invalid size {Width}x{Height}, expected 64x64", flowmapPath, bitmap.Width, bitmap.Height);
            return CreateBlankFlowmap();
        }

        return FlowmapData.Decode(bitmap);
    }

    /// <summary>Creates a blank flowmap from the embedded template asset.</summary>
    public FlowmapData CreateBlankFlowmap() {
        try {
            using var stream = AssetLoader.Open(FlowTemplateUri);
            var bitmap = ddsDecoder.Decode(stream);
            if (bitmap is not null) return FlowmapData.Decode(bitmap);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to load flowmap template: {Message}", e.Message);
        }

        return new FlowmapData();
    }

    /// <summary>
    /// Saves a flowmap as DDS (B8G8R8A8_UNORM) into the active data source, converting the painted
    /// texture with the given texture processor integration.
    /// </summary>
    public async Task<bool> SaveFlowmap(
        FlowmapData flowmapData,
        P2Int position,
        ITextureProcessor textureProcessor,
        ConfigValues textureProcessorConfig) {
        string? tempDirectory = null;
        try {
            tempDirectory = Path.Combine(Path.GetTempPath(), "WaterFlowPainter", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);

            var pngPath = Path.Combine(tempDirectory, "Flow.png");
            using (var bitmap = flowmapData.Encode()) {
                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = File.Create(pngPath);
                data.SaveTo(stream);
            }

            var result = await textureProcessor.ConvertToDds(pngPath, tempDirectory, TextureFormat.B8G8R8A8_UNORM, textureProcessorConfig, CancellationToken.None);
            if (!result.Succeeded || result.OutputFilePath is null) {
                logger.Here().Error("Failed to convert flowmap to DDS: {Message}", result.Message);
                return false;
            }

            var fileSystem = dataSourceService.ActiveDataSource.FileSystem;
            var directoryPath = fileSystem.Path.Combine(
                dataSourceService.ActiveDataSource.Path,
                "Textures",
                "Water",
                editorEnvironment.ActiveMod.ModKey.FileName);
            var targetPath = fileSystem.Path.Combine(directoryPath, $"Flow.{position.X}.{position.Y}.dds");

            fileSystem.Directory.CreateDirectory(directoryPath);
            fileSystem.File.Copy(result.OutputFilePath, targetPath, true);

            return true;
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to save flowmap of cell {X}.{Y}: {Message}", position.X, position.Y, e.Message);
            return false;
        } finally {
            if (tempDirectory is not null && Directory.Exists(tempDirectory)) {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}
