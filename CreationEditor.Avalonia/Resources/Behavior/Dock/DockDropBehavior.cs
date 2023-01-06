using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Extension;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DockDropBehavior : Behavior<Control> {
    public static readonly StyledProperty<DockContainerVM?> DockContainerProperty = AvaloniaProperty.Register<Control, DockContainerVM?>(nameof(DockContainer));

    public DockContainerVM? DockContainer {
        get => GetValue(DockContainerProperty);
        set => SetValue(DockContainerProperty, value);
    }
    
    protected override void OnAttached() {
        base.OnAttached();
        
        if (AssociatedObject == null) return;

        AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        
        AssociatedObject.SetValue(DragDrop.AllowDropProperty, true);
    }

    private void DragLeave(object? sender, DragEventArgs e) {
        if (!CanDock(sender, e, out var dragData, out var control)) return;
        
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

    private void DragOver(object? sender, DragEventArgs e) {
        if (!CanDock(sender, e, out var dragData, out var control)) return;
        
        // Check if the dock type changed or if we aren't the currently previewed object
        var dock = GetDockType(e, control);
        if (dragData.Dock != dock
         || !ReferenceEquals(dragData.Preview, AssociatedObject)) {   
            // Handle preview
            if (control is IDockPreview dockPreview) {
                dragData.Preview?.HidePreview();

                dragData.Preview = dockPreview;
                dragData.Dock = dock;
                dockPreview.ShowPreview(dock);
            }
        }
    }

    private void Drop(object? sender, DragEventArgs e) {
        if (!CanDock(sender, e, out var dragData, out var control)) return;
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
    private bool CanDock(object? sender, DragEventArgs e, [MaybeNullWhen(false)] out DockDragData outDockDragData, [MaybeNullWhen(false)] out Control outControl) {
        outDockDragData = null;
        outControl = null;
        
        // Check null
        if (DockContainer == null || sender == null) return false;

        // Check control
        if (sender is not Control control) return false;

        // Check if we're the nearest dock preview ancestor
        if (e.Source is not Control source) return false;
        if (!ReferenceEquals(source.FindAncestorOfType<IDockPreview>(), AssociatedObject)) return false;

        // Check drag data
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        if (e.Data?.Contains(nameof(DockDragData)) is not true) return false;
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        var data = e.Data?.Get(nameof(DockDragData));
        if (data is not DockDragData dragData) return false;
        
        // Don't allow to drop on ourselves
        if (dragData.Item.DockParent == DockContainer) return false;
        
        outControl = control;
        outDockDragData = dragData;
        return true;
    }
    
    private static Dock GetDockType(DragEventArgs e, IVisual visual) {
        var position = e.GetPosition(visual);
        
        var dockPositions = new List<(double Distance, Dock Dock)> {
            (position.Distance(visual.Bounds.TopLeft, visual.Bounds.BottomLeft - visual.Bounds.TopLeft), Dock.Left),
            (position.Distance(visual.Bounds.TopRight, visual.Bounds.BottomRight - visual.Bounds.TopRight), Dock.Right),
            (position.Distance(visual.Bounds.TopLeft, visual.Bounds.TopRight - visual.Bounds.TopLeft), Dock.Top),
            (position.Distance(visual.Bounds.BottomLeft, visual.Bounds.BottomRight - visual.Bounds.BottomLeft), Dock.Bottom)
        };

        return dockPositions.MinBy(x => x.Distance).Dock;
    }
}