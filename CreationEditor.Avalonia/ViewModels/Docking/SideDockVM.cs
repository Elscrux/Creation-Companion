using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Panels at any side that are organized in a tab view
/// </summary>
public sealed class SideDockVM(DockContainerVM dockParent) : TabbedDockVM(dockParent) {
    public override DockMode DockMode => DockMode.Side;

    protected override void Unfocus() {
        ActiveTab?.IsSelected = false;
        ActiveTab = null;
    }
}
