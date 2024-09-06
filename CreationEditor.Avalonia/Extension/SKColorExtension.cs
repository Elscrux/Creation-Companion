using System.Drawing;
using SkiaSharp;
namespace CreationEditor.Avalonia;

public static class SKColorExtension {
    public static float ToGrayscale(this SKColor color) {
        return (color.Red + color.Green + color.Blue) / 3.0f / 255.0f;
    }

    public static SKColor GetAverageColor(this IReadOnlyList<SKColor> colors) {
        return new SKColor(
            (byte) colors.Average(c => c.Red),
            (byte) colors.Average(c => c.Green),
            (byte) colors.Average(c => c.Blue),
            (byte) colors.Average(c => c.Alpha)
        );
    }

    public static Color ToSystemDrawingColor(this SKColor color) {
        return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
    }
}
