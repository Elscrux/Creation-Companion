using Avalonia;
namespace CreationEditor.Avalonia.Models.Settings.View;

public interface IViewModeTemplate {
    public ViewMode ViewMode { get; }
    public Dictionary<string, Thickness> Spacings { get; }
}
