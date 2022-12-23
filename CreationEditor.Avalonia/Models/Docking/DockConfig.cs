using Avalonia.Controls;
namespace CreationEditor.Avalonia.Models.Docking;

public class DockConfig {
    public string Header { get; init; } = string.Empty;
    public DockType DockType { get; init; } = DockType.Layout;
    public Dock Dock { get; init; } = Dock.Left;
    public GridLength Size { get; init; } = new(1, GridUnitType.Star);
}
