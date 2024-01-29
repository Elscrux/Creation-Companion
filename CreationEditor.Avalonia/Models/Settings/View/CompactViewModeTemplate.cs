using Avalonia;
namespace CreationEditor.Avalonia.Models.Settings.View;

public class CompactViewModeTemplate : IViewModeTemplate {
    public ViewMode ViewMode => ViewMode.Compact;

    public Dictionary<string, Thickness> Spacings { get; } = new() {
        { "DataGridMarginLowSpacing", new Thickness(0, 0) },
        { "DataGridMarginMediumSpacing", new Thickness(1, 3) },
        { "DataGridMarginHighSpacing", new Thickness(2, 6) },
    };

    public Dictionary<string, double> FontSizes { get; } = new() {
        { "TextBlockFontSizeHeading1", 24 },
        { "TextBlockFontSizeHeading2", 20 },
        { "TextBlockFontSizeHeading3", 16 },
        { "DataGridFontSizeSmall", 12 },
        { "DataGridFontSizeMedium", 14 },
        { "DataGridFontSizeLarge", 16 },
        { "ComboBoxFontSizeSmall", 6.25 },
        { "ComboBoxFontSizeMedium", 9 },
        { "ComboBoxFontSizeLarge", 12 },
    };

    public Dictionary<string, double> MaxHeights { get; } = new() {
        { "DataGridMaxHeightSmall", 26 },
        { "DataGridMaxHeightMedium", 28 },
        { "DataGridMaxHeightLarge", 32 },
    };
}
