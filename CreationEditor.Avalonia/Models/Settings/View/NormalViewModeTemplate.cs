using Avalonia;
namespace CreationEditor.Avalonia.Models.Settings.View;

public class NormalViewModeTemplate : IViewModeTemplate {
    public ViewMode ViewMode => ViewMode.Normal;

    public Dictionary<string, Thickness> Spacings { get; } = new() {
        { "DataGridMarginLowSpacing", new Thickness(1, 3) },
        { "DataGridMarginMediumSpacing", new Thickness(2, 6) },
        { "DataGridMarginHighSpacing", new Thickness(3, 12) },
    };
}
