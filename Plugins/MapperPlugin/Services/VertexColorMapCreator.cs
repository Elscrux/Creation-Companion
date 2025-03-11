using Avalonia;
using Avalonia.Media.Imaging;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using SkiaSharp;
using Size = Avalonia.Size;
namespace MapperPlugin.Services;

using BitmapWithTopLeftCell = (SKBitmap Bitmap, P2Int TopLeftCell);

public sealed class VertexColorMapCreator {
    private readonly record struct ColorEntry(P2Int Position, P3UInt8 Color);

    private const int CellVertexColor = 33;

    private readonly Dictionary<FormKey, BitmapWithTopLeftCell> _bitmapCache = [];

    private static IEnumerable<ColorEntry> GetVertexColor(IWorldspaceGetter worldspace) {
        foreach (var cell in worldspace.EnumerateMajorRecords<ICellGetter>()) {
            if (cell.Landscape is null) continue;
            if (cell.Grid is null) continue;
            if (cell.Landscape.VertexColors is null) continue;

            foreach (var vertexColor in cell.Landscape.VertexColors) {
                yield return new ColorEntry((cell.Grid.Point * CellVertexColor) + vertexColor.Key, vertexColor.Value);
            }
        }
    }

    private static BitmapWithTopLeftCell CreateVertexColorBitmap(IWorldspaceGetter worldspace) {
        var vertexColor = GetVertexColor(worldspace).ToArray();

        int minX = 0, maxX = 0, minY = 0, maxY = 0;

        foreach (var (position, _) in vertexColor) {
            minX = Math.Min(minX, position.X);
            maxX = Math.Max(maxX, position.X);
            minY = Math.Min(minY, position.Y);
            maxY = Math.Max(maxY, position.Y);
        }

        var width = maxX - minX + 2;
        var height = maxY - minY + 2;

        var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);

        foreach (var (position, color) in vertexColor) {
            bitmap.SetPixel(position.X - minX, maxY - position.Y, new SKColor(color.X, color.Y, color.Z));
        }

        return (bitmap, new P2Int(minX / CellVertexColor, maxY / CellVertexColor));
    }

    public Bitmap ScaleForGrid(IWorldspaceGetter worldspace, Size imageSize, int leftCell, int rightCell, int topCell, int bottomCell) {
        var (bitmap, topLeftCell) = _bitmapCache.GetOrAdd(worldspace.FormKey, () => CreateVertexColorBitmap(worldspace));

        // Crop the bitmap to the desired area
        var distanceToCellBorders = (new P2Int(leftCell, topCell) - topLeftCell) * CellVertexColor;
        var rectangleTopLeft = new SKPointI(distanceToCellBorders.X, -distanceToCellBorders.Y);
        var rectangleSize = new SKSizeI((rightCell - leftCell) * CellVertexColor, (topCell - bottomCell) * CellVertexColor);
        var croppedBitmap = new SKBitmap(rectangleSize.Width, rectangleSize.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        bitmap.ExtractSubset(croppedBitmap, SKRectI.Create(rectangleTopLeft, rectangleSize));

        // Convert the bitmap to an Avalonia bitmap
        var data = croppedBitmap.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = data.AsStream();
        var avaloniaBitmap = new Bitmap(stream);

        // Scale the bitmap to the desired size
        return avaloniaBitmap.CreateScaledBitmap(new PixelSize((int) imageSize.Width, (int) imageSize.Height));
    }
}
