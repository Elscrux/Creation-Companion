using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
namespace CreationEditor.Avalonia.Constants;

public static class StandardBrushes {
    public static IBrush? HighlightBrush => GetBrush("SystemAccentColor");

    public static IBrush? BackgroundBrush => GetBrush("SolidBackgroundFillColorTertiary");

    public static ISolidColorBrush ValidBrush => Brushes.ForestGreen;
    public static ISolidColorBrush InvalidBrush => Brushes.IndianRed;

    public static SolidColorBrush? GetBrush(string dynamicColorKey) =>
        Application.Current is not null
     && Application.Current.TryFindResource(dynamicColorKey, Application.Current.ActualThemeVariant, out var obj)
            ? obj switch {
                Color color => new SolidColorBrush(color),
                SolidColorBrush brush => brush,
                _ => null
            }
            : null;
}
