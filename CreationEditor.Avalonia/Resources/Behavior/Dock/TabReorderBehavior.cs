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
        if (movingTabIndex == -1) {
            // The dragged item is not part of this tab container (e.g. dragged from a floating dock)
            // - dock it as a new tab instead of reordering
            base.Drop(sender, e);
            return;
        }

        var selectedTabIndex = tab.DockContainer.Tabs.IndexOf(tab.DockedItem);
        if (selectedTabIndex == -1) return;

        switch (dragData.Zone) {
            case DockDropZone.Left:
                tab.DockContainer
                    .Move(movingTabIndex,
                        selectedTabIndex == 0
                            ? 0
                            : selectedTabIndex);
                break;
            case DockDropZone.Right:
                tab.DockContainer
                    .Move(movingTabIndex,
                        selectedTabIndex + 1 == tab.DockContainer.Tabs.Count
                            ? selectedTabIndex
                            : selectedTabIndex + 1);
                break;
        }
    }

    protected override DockDropZone GetDropZone(DragEventArgs e, Visual visual) {
        var position = e.GetPosition(visual);

        return position.X < visual.Bounds.Width / 2
            ? DockDropZone.Left
            : DockDropZone.Right;
    }

    protected override bool CheckSelf(DockDragData dragData, Control control) {
        return dragData.Item == control.DataContext;
    }
}
