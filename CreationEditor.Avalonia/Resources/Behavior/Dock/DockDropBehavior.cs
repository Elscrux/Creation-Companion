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
            dragData.Dock = null;
        }
    }

    protected virtual void DragOver(object? sender, DragEventArgs e) {
        if (!CanDock<Control>(sender, e, out var dragData, out var control)) return;

        // Check if the dock type changed or if we aren't the currently previewed object
        var dock = GetDockType(e, control);
        if (dragData.Dock == dock && ReferenceEquals(dragData.Preview, AssociatedObject)) return;

        // Handle preview
        if (control is IDockPreview dockPreview) {
            dragData.Preview?.HidePreview();

            dragData.Preview = dockPreview;
            dragData.Dock = dock;
            dockPreview.ShowPreview(dock);
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
            .Remove(dragData.Item.DockParent, dragData.Item)
            .Add(DockContainer, dragData.Item, new DockConfig { Dock = GetDockType(e, control) })
            .StartReporting()
            .FinishEdit();
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

        // Check drag data
        if (e.Data is null) return false;
        if (!e.Data.Contains(nameof(DockDragData))) return false;

        var data = e.Data.Get(nameof(DockDragData));
        if (data is not DockDragData dragData) return false;

        // Don't allow to drop on ourselves
        if (CheckSelf(dragData, control)) return false;

        outControl = control;
        outDockDragData = dragData;
        return true;
    }

    protected virtual bool CheckSelf(DockDragData dragData, Control control) {
        return dragData.Item.DockParent == DockContainer;
    }

    protected virtual Dock GetDockType(DragEventArgs e, Visual visual) {
        var position = e.GetPosition(visual);

        var dockPositions = new List<(double Distance, Dock Dock)> {
            (position.Distance(new Point(0, 0), new Point(0, visual.Bounds.Height)), Dock.Left),
            (position.Distance(new Point(visual.Bounds.Width, 0), new Point(0, visual.Bounds.Height)), Dock.Right),
            (position.Distance(new Point(0, 0), new Point(visual.Bounds.Width, 0)), Dock.Top),
            (position.Distance(new Point(0, visual.Bounds.Height), new Point(visual.Bounds.Width, 0)), Dock.Bottom),
        };

        return dockPositions.MinBy(x => x.Distance).Dock;
    }
}
