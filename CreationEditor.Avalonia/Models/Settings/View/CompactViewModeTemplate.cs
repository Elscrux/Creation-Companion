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
        { "DataGridFontSizeSmall", 8 },
        { "DataGridFontSizeMedium", 10 },
        { "DataGridFontSizeLarge", 12 },
        { "ComboBoxFontSizeSmall", 6.25 },
        { "ComboBoxFontSizeMedium", 9 },
        { "ComboBoxFontSizeLarge", 12 },
    };

    public Dictionary<string, double> MaxHeights { get; } = new() {
        { "DataGridMaxHeightSmall", 20 },
        { "DataGridMaxHeightMedium", 24 },
        { "DataGridMaxHeightLarge", 28 },
        { "ComboBoxMaxHeightSmall", 20 },
        { "ComboBoxMaxHeightMedium", 24 },
        { "ComboBoxMaxHeightLarge", 28 },
    };
}
