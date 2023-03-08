using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
namespace CreationEditor.Avalonia.Constants;

public static class StandardBrushes {
    public static IBrush? ValidBrush =>
        Application.Current != null
     && Application.Current.TryFindResource("SystemAccentColor", out var obj)
     && obj is Color color
            ? new SolidColorBrush(color)
            : null;

    public static IBrush InvalidBrush => Brushes.Red;
}
