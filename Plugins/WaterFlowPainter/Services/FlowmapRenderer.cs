using Noggog;
using SkiaSharp;
using WaterFlowPainter.Model;
namespace WaterFlowPainter.Services;

/// <summary>
/// Renders flowmap data into preview bitmaps for the worldspace overview.
/// </summary>
public sealed class FlowmapRenderer {
    /// <summary>Resolution of the flowmap data itself.</summary>
    public const int FlowmapPixelSize = FlowmapData.Size;

    /// <summary>A cell with its flowmap and optional land mask, positioned in cell grid coordinates.</summary>
    public sealed record RenderCell(P2Int Position, FlowmapData Flowmap, SKBitmap? LandMask);

    /// <summary>
    /// Creates a composite flow preview of all cells in the given cell range. Each pixel is color
    /// coded by flow direction (hue) and speed (brightness); cells without flow are transparent.
    /// Optionally draws flow direction arrows and cell border lines. The flowmap of each cell is
    /// decimated to <paramref name="cellPixelSize" /> pixels to bound the overview resolution.
    /// </summary>
    public SKBitmap CreateFlowPreview(
        IEnumerable<RenderCell> cells,
        int leftCell,
        int bottomCell,
        int rightCell,
        int topCell,
        bool showArrows,
        int cellPixelSize) {
        var width = (rightCell - leftCell) * cellPixelSize;
        var height = (topCell - bottomCell) * cellPixelSize;
        var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);

        using var canvas = new SKCanvas(bitmap);
        foreach (var cell in cells) {
            var x = (cell.Position.X - leftCell) * cellPixelSize;
            var y = (topCell - cell.Position.Y) * cellPixelSize;
            if (x < -cellPixelSize || y < -cellPixelSize || x >= width || y >= height) continue;

            DrawFlowmap(canvas, cell.Flowmap, x, y, showArrows, cellPixelSize);
        }

        DrawCellBorders(canvas, width, height, cellPixelSize);
        return bitmap;
    }

    /// <summary>
    /// Creates a composite overlay of the land masks (landscape above water level) of all cells
    /// in the given cell range.
    /// </summary>
    public static SKBitmap CreateHeightmapOverlay(IEnumerable<RenderCell> cells, int leftCell, int bottomCell, int rightCell, int topCell, int cellPixelSize) {
        var width = (rightCell - leftCell) * cellPixelSize;
        var height = (topCell - bottomCell) * cellPixelSize;
        var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);

        using var canvas = new SKCanvas(bitmap);
        foreach (var cell in cells) {
            if (cell.LandMask is null) continue;

            var x = (cell.Position.X - leftCell) * cellPixelSize;
            var y = (topCell - cell.Position.Y) * cellPixelSize;
            if (x < -cellPixelSize || y < -cellPixelSize || x >= width || y >= height) continue;

            canvas.DrawBitmap(cell.LandMask, new SKRect(x, y, x + cellPixelSize, y + cellPixelSize));
        }

        return bitmap;
    }

    /// <summary>Draws a single flowmap color coded at the given pixel offset, decimated to <paramref name="cellPixelSize" />.</summary>
    public void DrawFlowmap(SKCanvas canvas, FlowmapData flowmap, int offsetX, int offsetY, bool showArrows, int cellPixelSize = FlowmapPixelSize) {
        var step = (float) FlowmapData.Size / cellPixelSize;
        using var paint = new SKPaint();
        for (var y = 0; y < cellPixelSize; y++) {
            for (var x = 0; x < cellPixelSize; x++) {
                // Sample the center of the block this overview pixel represents.
                // Flowmap row 0 is the south edge of the cell, screen row 0 is north.
                var sampleX = Math.Min(FlowmapData.Size - 1, (int) ((x + 0.5f) * step));
                var sampleY = Math.Min(FlowmapData.Size - 1, FlowmapData.Size - 1 - (int) ((y + 0.5f) * step));
                var index = FlowmapData.Index(sampleX, sampleY);
                var speed = flowmap.Speed[index];
                if (speed <= 0.01f) continue;

                var hue = flowmap.Direction[index] / MathF.Tau * 360;
                paint.Color = SKColor.FromHsv(hue, 0.9f, 0.35f + 0.65f * speed).WithAlpha(220);
                canvas.DrawRect(offsetX + x, offsetY + y, 1, 1, paint);
            }
        }

        if (showArrows) DrawArrows(canvas, flowmap, offsetX, offsetY, cellPixelSize);
    }

    private static void DrawArrows(SKCanvas canvas, FlowmapData flowmap, int offsetX, int offsetY, int cellPixelSize) {
        var scale = (float) cellPixelSize / FlowmapData.Size;
        const int arrowStep = 8;
        using var paint = new SKPaint {
            Color = SKColors.White.WithAlpha(200),
            StrokeWidth = MathF.Max(1, scale),
            IsAntialias = true,
        };

        for (var y = arrowStep / 2; y < FlowmapData.Size; y += arrowStep) {
            for (var x = arrowStep / 2; x < FlowmapData.Size; x += arrowStep) {
                var index = FlowmapData.Index(x, y);
                var speed = flowmap.Speed[index];
                if (speed <= 0.05f) continue;

                var direction = flowmap.Direction[index];
                // Screen Y grows downwards, so invert the Y component to point north upwards
                var dx = MathF.Cos(direction);
                var dy = -MathF.Sin(direction);
                var length = (1.5f + speed * 4f) * scale;

                // Flowmap row 0 is the south edge, so flip Y for the screen position
                var centerX = offsetX + (x + 0.5f) * scale;
                var centerY = offsetY + (FlowmapData.Size - y - 0.5f) * scale;
                var endX = centerX + dx * length;
                var endY = centerY + dy * length;

                canvas.DrawLine(centerX, centerY, endX, endY, paint);

                // Arrow head
                var headAngle = 0.6f;
                var headLength = 2.5f * scale;
                canvas.DrawLine(
                    endX,
                    endY,
                    endX - (dx * MathF.Cos(headAngle) - dy * MathF.Sin(headAngle)) * headLength,
                    endY - (dx * MathF.Sin(headAngle) + dy * MathF.Cos(headAngle)) * headLength,
                    paint);
                canvas.DrawLine(
                    endX,
                    endY,
                    endX - (dx * MathF.Cos(-headAngle) - dy * MathF.Sin(-headAngle)) * headLength,
                    endY - (dx * MathF.Sin(-headAngle) + dy * MathF.Cos(-headAngle)) * headLength,
                    paint);
            }
        }
    }

    private static void DrawCellBorders(SKCanvas canvas, int width, int height, int cellPixelSize) {
        using var paint = new SKPaint {
            Color = SKColors.Black.WithAlpha(90),
            StrokeWidth = 1,
        };

        for (var x = 0; x <= width; x += cellPixelSize) {
            canvas.DrawLine(x + 0.5f, 0, x + 0.5f, height, paint);
        }

        for (var y = 0; y <= height; y += cellPixelSize) {
            canvas.DrawLine(0, y + 0.5f, width, y + 0.5f, paint);
        }
    }

    /// <summary>Converts a SKBitmap into an encoded PNG stream, ready to be used as Avalonia image.</summary>
    public static MemoryStream EncodePng(SKBitmap bitmap) {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var stream = new MemoryStream();
        data.SaveTo(stream);
        stream.Position = 0;
        return stream;
    }
}
