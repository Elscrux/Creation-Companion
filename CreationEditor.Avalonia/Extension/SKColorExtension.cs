using System.Drawing;
using SkiaSharp;
namespace CreationEditor.Avalonia;

public static class SKColorExtension {
    extension(SKColor color) {
        public float ToGrayscale() {
            return (color.Red + color.Green + color.Blue) / 3.0f / 255.0f;
        }
        public Color ToSystemDrawingColor() {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }
    }

    extension(IReadOnlyList<SKColor> colors) {
        public SKColor GetAverageColor() {
            return new SKColor(
                (byte) colors.Average(c => c.Red),
                (byte) colors.Average(c => c.Green),
                (byte) colors.Average(c => c.Blue),
                (byte) colors.Average(c => c.Alpha)
            );
        }
    }
}
