using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Behavior;

public class DockDropBehavior : Behavior<Control> {
    public static readonly StyledProperty<DockContainerVM?> DockContainerProperty =
        AvaloniaProperty.Register<Control, DockContainerVM?>(nameof(DockContainer));

    public DockContainerVM? DockContainer {
        get => GetValue(DockContainerProperty);
        set => SetValue(DockContainerProperty, value);
    }

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null) return;

        AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, DragLeave);

        AssociatedObject.SetValue(DragDrop.AllowDropProperty, true);
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null) return;

        AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
        AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
    }

    protected virtual void DragLeave(object? sender, DragEventArgs e) {
        if (!CanDock<Control>(sender, e, out var dragData, out var control)) return;

        // Check if we are out of bounds of our control
        if (control is IDockPreview dockPreview
         && dockPreview == dragData.Preview
         && !control.Bounds.ContainsExclusive(e.GetPosition(control))) {
            // Handle preview
            dragData.Preview?.HidePreview();

            dragData.Preview = null;
            dragData.Zone = null;
        }
    }

    protected virtual void DragOver(object? sender, DragEventArgs e) {
        if (!CanDock<Control>(sender, e, out var dragData, out var control)) return;

        // Check if the drop zone changed or if we aren't the currently previewed object
        var zone = GetDropZone(e, control);
        if (dragData.Zone == zone && ReferenceEquals(dragData.Preview, AssociatedObject)) return;

        // Handle preview
        if (control is IDockPreview dockPreview) {
            dragData.Preview?.HidePreview();

            dragData.Preview = dockPreview;
            dragData.Zone = zone;

            if (zone == DockDropZone.Center) {
                // Dropping in the middle area docks the items as documents,
                // covering the entire target
                dockPreview.ShowDocumentPreview();
            } else {
                dockPreview.ShowPreview((Dock) zone);
            }
        }
    }

    protected virtual void Drop(object? sender, DragEventArgs e) {
        if (!CanDock<Control>(sender, e, out var dragData, out var control)) return;

        e.Handled = true;

        // Handle preview
        dragData.Preview?.HidePreview();

        // Handle layout change
        (DockContainer as IDockObject).DockRoot
            .StartEdit()
            .StopReporting()
            .Do(() => PerformDrop(dragData, control, GetDropZone(e, control)))
            .StartReporting()
            .FinishEdit();
    }

    /// <summary>
    /// Removes all dragged items from their current parent and docks them into the target container.
    /// </summary>
    protected void PerformDrop(DockDragData dragData, Control control, DockDropZone zone) {
        if (DockContainer is null) return;

        var items = dragData.Items.Count > 0 ? dragData.Items : [dragData.Item];
        foreach (var item in items) {
            item.DockParent.Remove(item);
        }

        // Dropping in the middle area converts the target into a document dock:
        // layout docks are converted into document docks containing the existing item
        // as first and the dragged items as following tabs, tabbed docks receive the
        // dragged items as tabs
        if (zone == DockDropZone.Center) {
            if (DockContainer is LayoutDockVM layoutDock
             && control is DockedControl dockedControl) {
                layoutDock.ConvertToDocumentDock(dockedControl, items);
                return;
            }

            var tabConfig = new DockConfig {
                DockMode = DockMode.Document,
            };
            foreach (var item in items) {
                DockContainer.Add(item, tabConfig);
            }

            return;
        }

        // Edge drops split the target at that side
        var config = new DockConfig { Dock = (Dock) zone };
        foreach (var item in items) {
            DockContainer.Add(item, config);
        }
    }

    /// <summary>
    /// Determines the drop zone of the given position inside the target:
    /// the middle area docks as documents, the outer areas split at the closest edge.
    /// </summary>
    protected virtual DockDropZone GetDropZone(DragEventArgs e, Visual visual) {
        var position = e.GetPosition(visual);
        var width = visual.Bounds.Width;
        var height = visual.Bounds.Height;

        // Dropping in the middle area docks as document (tab) instead of splitting
        if (position.X > width * 0.3 && position.X < width * 0.7
         && position.Y > height * 0.3 && position.Y < height * 0.7) {
            return DockDropZone.Center;
        }

        var dockPositions = new List<(double Distance, Dock Dock)> {
            (position.Distance(new Point(0, 0), new Point(0, height)), Dock.Left),
            (position.Distance(new Point(width, 0), new Point(0, height)), Dock.Right),
            (position.Distance(new Point(0, 0), new Point(width, 0)), Dock.Top),
            (position.Distance(new Point(0, height), new Point(width, 0)), Dock.Bottom),
        };

        return (DockDropZone) dockPositions.MinBy(x => x.Distance).Dock;
    }

    [MemberNotNullWhen(true, nameof(DockContainer))]
    protected bool CanDock<T>(
        object? sender,
        DragEventArgs e,
        [MaybeNullWhen(false)] out DockDragData outDockDragData,
        [MaybeNullWhen(false)] out T outControl)
        where T : Control {
        outDockDragData = null;
        outControl = null;

        // Check null
        if (DockContainer is null || sender is null) return false;

        // Check control
        if (sender is not T control) return false;

        // Check if we're the nearest dock preview ancestor
        if (e.Source is not Control source) return false;
        if (!ReferenceEquals(source.FindAncestorOfType<IDockPreview>(), AssociatedObject)) return false;

        // Get drag data
        if (!DockDragContext.TryGet(e, out var dragData)) return false;

        // Don't allow to drop on ourselves
        if (CheckSelf(dragData, control)) return false;

        outControl = control;
        outDockDragData = dragData;
        return true;
    }

    protected virtual bool CheckSelf(DockDragData dragData, Control control) => dragData.Item.DockParent == DockContainer;
}
