using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DockDragBehavior : Behavior<Control> {
    private readonly DragStartHandler _dragStartHandler = new(Drag);

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null) return;

        _dragStartHandler.Register(AssociatedObject);
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null) return;

        _dragStartHandler.Unregister(AssociatedObject);
    }

    private static async Task Drag(object? sender, object? identifier, PointerEventArgs e) {
        if (sender is not Control { DataContext: IDockedItem dockedItem } control) return;

        var dragData = new DockDragData {
            Item = dockedItem,
            Items = [dockedItem],
        };
        var dataTransfer = IDataTransfer.Create(dragData);

        SetDragging(true);
        PixelPoint? dropScreenPosition;
        using (DockDragContext.Begin(dragData)) {
            dropScreenPosition = await DoDragDrop(control, e, dataTransfer);
        }
        SetDragging(false);

        if (dropScreenPosition is null) {
            // Drag ended inside the window without a drop target - just hide the preview
            dragData.Preview?.HidePreview();
            return;
        }

        // Drag was released outside of the window without a drop target - float the item in a new window
        Float(dockedItem, dropScreenPosition.Value);

        void SetDragging(bool state) {
            dockedItem.DockRoot.SetEditMode(state);
        }
    }

    private static void Float(IDockedItem dockedItem, PixelPoint screenPosition) {
        var size = dockedItem.Control.Bounds.Size;
        if (size.Width < 1) size = new Size(400, 300);

        // Place the title bar of the floating window under the cursor,
        // converting the device independent offsets to pixels for multi DPI setups
        var position = screenPosition;
        if (TopLevel.GetTopLevel(dockedItem.Control) is {} topLevel) {
            var origin = topLevel.PointToScreen(new Point(0, 0));
            var halfWidth = topLevel.PointToScreen(new Point(size.Width / 2, 0));
            var titleOffset = topLevel.PointToScreen(new Point(0, 13));

            position = new PixelPoint(
                screenPosition.X - (halfWidth.X - origin.X),
                screenPosition.Y - (titleOffset.Y - origin.Y));
        }

        dockedItem.DockRoot.Float(dockedItem, position, size);
    }

    /// <summary>
    /// Starts the drag operation and determines where the drag was released.
    /// </summary>
    /// <returns>
    /// The screen position where the drag was released, if it was released outside of the
    /// window without being accepted by a drop target. Null otherwise.
    /// </returns>
    private static async Task<PixelPoint?> DoDragDrop(Control control, PointerEventArgs e, IDataTransfer dataTransfer) {
        var topLevel = TopLevel.GetTopLevel(control);
        PixelPoint? lastKnownPosition = null;
        var leftWindow = false;

        void OnDragOver(object? s, RoutedEventArgs args) {
            if (args is not DragEventArgs dragArgs) return;

            lastKnownPosition = topLevel.PointToScreen(dragArgs.GetPosition(topLevel));
        }

        void OnDragLeave(object? s, RoutedEventArgs args) {
            leftWindow = true;
            if (args is not DragEventArgs dragArgs || lastKnownPosition is not null) return;

            lastKnownPosition = topLevel.PointToScreen(dragArgs.GetPosition(topLevel));
        }

        if (topLevel is not null) {
            topLevel.AddHandler(DragDrop.DragOverEvent, OnDragOver, RoutingStrategies.Bubble, true);
            topLevel.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave, RoutingStrategies.Bubble, true);
        }

        try {
            var position = e.GetCurrentPoint(control).Position;
            var args = new PointerPressedEventArgs(
                e.Source,
                e.Pointer,
                control,
                position,
                e.Timestamp,
                e.GetCurrentPoint(control).Properties,
                e.KeyModifiers);

            var result = await DragDrop.DoDragDropAsync(args, dataTransfer, DragDropEffects.Move);
            if (result != DragDropEffects.None || topLevel is null) return null;

            // The drag was not accepted by any drop target.
            // Float the item only if the cursor was released outside of the window the drag started in.
            var cursorPosition = CursorHelper.GetCursorScreenPosition();
            if (cursorPosition is not null) {
                return !CursorHelper.IsInsideWindow(topLevel, cursorPosition.Value) ? cursorPosition : null;
            }

            // Fallback without OS cursor access: use the tracked drag positions
            return leftWindow && lastKnownPosition is not null ? lastKnownPosition : null;
        } finally {
            if (topLevel is not null) {
                topLevel.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
                topLevel.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            }
        }
    }

}
