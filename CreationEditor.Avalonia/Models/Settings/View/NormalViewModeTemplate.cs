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
        { "DataGridFontSizeSmall", 10 },
        { "DataGridFontSizeMedium", 12 },
        { "DataGridFontSizeLarge", 15 },
        { "ComboBoxFontSizeSmall", 7.75 },
        { "ComboBoxFontSizeMedium", 10 },
        { "ComboBoxFontSizeLarge", 14 },
    };

    public Dictionary<string, double> MaxHeights { get; } = new() {
        { "DataGridMaxHeightSmall", 22 },
        { "DataGridMaxHeightMedium", 26 },
        { "DataGridMaxHeightLarge", 32 },
        { "ComboBoxMaxHeightSmall", 22 },
        { "ComboBoxMaxHeightMedium", 26 },
        { "ComboBoxMaxHeightLarge", 32 },
    };
}
