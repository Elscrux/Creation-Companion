using Avalonia;
namespace CreationEditor.Avalonia.Models.Settings.View;

public class CompactViewModeTemplate : IViewModeTemplate {
    public ViewMode ViewMode => ViewMode.Compact;

    public Dictionary<string, Thickness> Spacings { get; } = new() {
        { "DataGridMarginLowSpacing", new Thickness(0, 0) },
        { "DataGridMarginMediumSpacing", new Thickness(1, 3) },
        { "DataGridMarginHighSpacing", new Thickness(2, 6) },
    };
}
