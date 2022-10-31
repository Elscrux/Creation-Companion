using AvalonDock.Layout;
namespace CreationEditor.WPF.ViewModels.Docking;

public struct DockingStatus {
    public DockingStatus() {}
    
    public bool IsDocked { get; init; } = true;
    public AnchorSide AnchorSide { get; init; } = AnchorSide.Left;
    public double Width { get; init; } = 0;
    public double Height { get; init; } = 0;
}
