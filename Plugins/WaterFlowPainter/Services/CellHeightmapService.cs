using Mutagen.Bethesda.Skyrim;
using Noggog;
using SkiaSharp;
using WaterFlowPainter.Model;
namespace WaterFlowPainter.Services;

/// <summary>
/// Parses the landscape of a cell and computes which parts are above the cell's water level.
/// </summary>
public sealed class CellHeightmapService {
    /// <summary>Number of landscape vertices per cell axis (inclusive).</summary>
    public const int VertexCount = 33;

    /// <summary>
    /// Builds a 64x64 alpha mask of the cell where land above the water level is opaque.
    /// Returns null if the cell has no landscape or no water level.
    /// </summary>
    public SKBitmap? GetLandMask(ICellGetter cell) {
        if (cell.Landscape?.VertexHeightMap is not {} heightMap) return null;

        var waterHeight = cell.WaterHeight;
        if (waterHeight is null or float.MaxValue) return null;

        var mask = new SKBitmap(FlowmapData.Size, FlowmapData.Size, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        var pixels = new byte[FlowmapData.PixelCount * 4];
        for (var y = 0; y < FlowmapData.Size; y++) {
            for (var x = 0; x < FlowmapData.Size; x++) {
                // Map the pixel to the nearest landscape vertex (33 vertices span the cell).
                // Mask row 0 is north, the vertex Y of the heightmap grows northwards from 0 = south.
                var vertexX = Math.Min(VertexCount - 1, (int) MathF.Round(x * (VertexCount - 1) / (float) (FlowmapData.Size - 1)));
                var vertexY = Math.Min(VertexCount - 1, VertexCount - 1 - (int) MathF.Round(y * (VertexCount - 1) / (float) (FlowmapData.Size - 1)));
                var height = heightMap.Offset + heightMap.HeightMap[new P2Int(vertexX, vertexY)] * 8f;

                var index = (y * FlowmapData.Size + x) * 4;
                if (height > waterHeight.Value) {
                    pixels[index + 0] = 60;
                    pixels[index + 1] = 90;
                    pixels[index + 2] = 120;
                    pixels[index + 3] = 160;
                }
            }
        }

        fixed (byte* source = pixels) {
            Buffer.MemoryCopy(source, (void*) mask.GetPixels(), pixels.Length, pixels.Length);
        }

        return mask;
    }
}
