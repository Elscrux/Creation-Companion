using Avalonia.Media;
namespace CreationEditor.Avalonia;

public static class ColorExtension {
    public static Color RandomColorRgb(Random? random = null) {
        random ??= Random.Shared;
        return Color.FromRgb(
            (byte) random.Next(0, 256),
            (byte) random.Next(0, 256),
            (byte) random.Next(0, 256));
    }
}
