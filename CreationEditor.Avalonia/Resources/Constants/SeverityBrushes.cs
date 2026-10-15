using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
namespace CreationEditor.Avalonia.Constants;

public static class SeverityBrushes {
    private static readonly Dictionary<Severity, SeverityBrushSet> Sets = new() {
        [Severity.CTD] = new SeverityBrushSet("SeverityCtdColor", Colors.IndianRed),
        [Severity.Error] = new SeverityBrushSet("SeverityErrorColor", Colors.Orange),
        [Severity.Warning] = new SeverityBrushSet("SeverityWarningColor", Colors.Gold),
        [Severity.Suggestion] = new SeverityBrushSet("SeveritySuggestionColor", Colors.CornflowerBlue),
        [Severity.None] = new SeverityBrushSet("SeverityInfoColor", Colors.ForestGreen),
    };

    public static IBrush Solid(Severity severity) => Get(severity).Solid;

    public static IBrush RowLip(Severity severity) => Get(severity).RowLip;

    public static IBrush RowFill(Severity severity) => Get(severity).RowFill;

    public static IBrush RowHoverStroke(Severity severity) => Get(severity).RowHoverStroke;

    public static IBrush RowHoverFill(Severity severity) => Get(severity).RowHoverFill;

    public static void Refresh() {
        foreach (var set in Sets.Values) {
            set.Refresh();
        }
    }

    private static SeverityBrushSet Get(Severity severity) =>
        Sets.TryGetValue(severity, out var set) ? set : Sets[Severity.None];
}

internal sealed class SeverityBrushSet {
    private static readonly Color RowFillRest = Color.FromArgb(0x22, 0x00, 0x00, 0x00);

    private readonly string _colorKey;

    public SolidColorBrush Solid { get; } = new();

    public LinearGradientBrush RowLip { get; } = new() {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
    };

    public RadialGradientBrush RowFill { get; } = new() {
        Center = new RelativePoint(0, 0, RelativeUnit.Relative),
        GradientOrigin = new RelativePoint(0, 0, RelativeUnit.Relative),
        RadiusX = new RelativeScalar(0.75, RelativeUnit.Relative),
        RadiusY = new RelativeScalar(3.5, RelativeUnit.Relative),
    };

    public LinearGradientBrush RowHoverStroke { get; } = new() {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(0.65, 2.0, RelativeUnit.Relative),
    };

    public RadialGradientBrush RowHoverFill { get; } = new() {
        Center = new RelativePoint(0, 0, RelativeUnit.Relative),
        GradientOrigin = new RelativePoint(0, 0, RelativeUnit.Relative),
        RadiusX = new RelativeScalar(0.75, RelativeUnit.Relative),
        RadiusY = new RelativeScalar(3.5, RelativeUnit.Relative),
    };

    public SeverityBrushSet(string colorKey, Color fallback) {
        _colorKey = colorKey;
        Apply(fallback);
    }

    public void Refresh() {
        if (Application.Current is {} app
         && app.TryFindResource(_colorKey, app.ActualThemeVariant, out var value)
         && value is Color color) {
            Apply(color);
        }
    }

    private void Apply(Color color) {
        Solid.Color = color;

        RowLip.GradientStops = [
            new GradientStop(Tint(color, 0xFF), 0),
            new GradientStop(Tint(color, 0xCC), 1),
        ];

        RowFill.GradientStops = [
            new GradientStop(Tint(color, 0x2E), 0),
            new GradientStop(Tint(color, 0x14), 0.35),
            new GradientStop(RowFillRest, 0.7),
            new GradientStop(RowFillRest, 1),
        ];

        RowHoverStroke.GradientStops = [
            new GradientStop(Tint(color, 0xFF), 0),
            new GradientStop(Tint(color, 0xC0), 0.25),
            new GradientStop(Tint(color, 0x70), 0.6),
            new GradientStop(Tint(color, 0x30), 1),
        ];

        RowHoverFill.GradientStops = [
            new GradientStop(Tint(color, 0x5A), 0),
            new GradientStop(Tint(color, 0x33), 0.4),
            new GradientStop(Tint(color, 0x14), 0.8),
            new GradientStop(RowFillRest, 1),
        ];
    }

    private static Color Tint(Color color, byte alpha) => Color.FromArgb(alpha, color.R, color.G, color.B);
}
