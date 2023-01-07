using Avalonia.Controls;
namespace CreationEditor.Avalonia.Models.Docking;

public sealed record DockConfig {
    public static readonly DockConfig Default = new();

    public DockInfo DockInfo { get; init; } = new();
    
    public DockMode DockMode { get; init; } = DockMode.Default;
    public Dock Dock { get; init; } = Dock.Left;
    public GridLength Size { get; init; } = new(1, GridUnitType.Star);
}