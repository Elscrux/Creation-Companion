﻿using Avalonia.Media;
namespace CreationEditor.Avalonia;

public static class ColorExtension {
    public static Color RandomColorRgb(Random? random = null) {
        random ??= Random.Shared;
        return Color.FromRgb(
            (byte) random.Next(0, 256),
            (byte) random.Next(0, 256),
            (byte) random.Next(0, 256));
    }

    public static System.Drawing.Color ToSystemDrawingsColor(this Color color) {
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}
