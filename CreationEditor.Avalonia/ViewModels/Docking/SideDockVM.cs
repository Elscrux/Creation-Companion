using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Panels at any side that are organized in a tab view
/// </summary>
public sealed class SideDockVM : TabbedDockVM {
    public override DockMode DockMode => DockMode.Side;
    public SideDockVM(DockContainerVM dockParent) : base(dockParent) {}

    protected override void Unfocus() {
        if (ActiveTab != null) ActiveTab.IsSelected = false;

        ActiveTab = null;
    }
}
