using Avalonia;
namespace CreationEditor.Avalonia.Models.Settings.View;

public class NormalViewModeTemplate : IViewModeTemplate {
    public ViewMode ViewMode => ViewMode.Normal;

    public Dictionary<string, Thickness> Spacings { get; } = new() {
        { "DataGridMarginLowSpacing", new Thickness(1, 3) },
        { "DataGridMarginMediumSpacing", new Thickness(2, 6) },
        { "DataGridMarginHighSpacing", new Thickness(3, 12) },
    };

    public Dictionary<string, double> FontSizes { get; } = new() {
        { "TextBlockFontSizeHeading1", 32 },
        { "TextBlockFontSizeHeading2", 28 },
        { "TextBlockFontSizeHeading3", 24 },
        { "DataGridFontSizeSmall", 14 },
        { "DataGridFontSizeMedium", 16 },
        { "DataGridFontSizeLarge", 20 },
        { "ComboBoxFontSizeSmall", 7.75 },
        { "ComboBoxFontSizeMedium", 10 },
        { "ComboBoxFontSizeLarge", 14 },
    };

    public Dictionary<string, double> MaxHeights { get; } = new() {
        { "DataGridMaxHeightSmall", 30 },
        { "DataGridMaxHeightMedium", 34 },
        { "DataGridMaxHeightLarge", 40 },
    };
}
