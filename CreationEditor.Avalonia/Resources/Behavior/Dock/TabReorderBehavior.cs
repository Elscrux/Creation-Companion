using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Behavior;

public sealed class TabReorderBehavior : DockDropBehavior {
    protected override void Drop(object? sender, DragEventArgs e) {
        if (!CanDock<Tab>(sender, e, out var dragData, out var tab)) return;

        e.Handled = true;

        // Handle preview
        dragData.Preview?.HidePreview();

        // Move tab
        if (tab.DockContainer is null) return;

        var movingTabIndex = tab.DockContainer.Tabs.IndexOf(dragData.Item);
        if (movingTabIndex == -1) return;

        var selectedTabIndex = tab.DockContainer.Tabs.IndexOf(tab.DockedItem);
        if (selectedTabIndex == -1) return;

        switch (dragData.Dock) {
            case Dock.Left:
                tab.DockContainer
                    .Move(movingTabIndex,
                        selectedTabIndex == 0
                            ? 0
                            : selectedTabIndex);
                break;
            case Dock.Right:
                tab.DockContainer
                    .Move(movingTabIndex,
                        selectedTabIndex + 1 == tab.DockContainer.Tabs.Count
                            ? selectedTabIndex
                            : selectedTabIndex + 1);
                break;
        }
    }

    protected override Dock GetDockType(DragEventArgs e, Visual visual) {
        var position = e.GetPosition(visual);

        var dockPositions = new List<(double Distance, Dock Dock)> {
            (position.Distance(new Point(0, 0), new Point(0, visual.Bounds.Height)), Dock.Left),
            (position.Distance(new Point(visual.Bounds.Width, 0), new Point(0, visual.Bounds.Height)), Dock.Right),
        };

        return dockPositions.MinBy(x => x.Distance).Dock;
    }

    protected override bool CheckSelf(DockDragData dragData, Control control) {
        return dragData.Item == control.DataContext;
    }
}
