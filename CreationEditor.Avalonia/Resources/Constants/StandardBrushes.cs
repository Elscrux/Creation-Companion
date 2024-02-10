using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
namespace CreationEditor.Avalonia.Constants;

public static class StandardBrushes {
    public static IBrush? ValidBrush =>
        Application.Current is not null
     && Application.Current.TryFindResource("SystemAccentColor", Application.Current.ActualThemeVariant, out var obj)
     && obj is Color color
            ? new SolidColorBrush(color)
            : null;

    public static IBrush? BackgroundBrush =>
        Application.Current is not null
     && Application.Current.TryFindResource("SolidBackgroundFillColorTertiary", Application.Current.ActualThemeVariant, out var obj)
     && obj is Color color
            ? new SolidColorBrush(color)
            : null;

    public static IBrush InvalidBrush => Brushes.Red;
}
