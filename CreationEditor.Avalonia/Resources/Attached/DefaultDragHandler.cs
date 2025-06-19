using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
namespace CreationEditor.Avalonia.Attached;

public sealed class DefaultDragHandler<T> : IDragHandler where T : Visual {
    public void BeforeDragDrop(object? sender, PointerEventArgs e, object? context) {
        if (context is not DragContext dragContext) return;
        if (sender is not Visual visual) return;

        var control = visual.FindAncestorOfType<T>(true);
        if (control is null) return;

        dragContext.Data[DragDropExtended.DataStartPosition] = e.GetPosition(control);
        dragContext.Data[DragDropExtended.DataSourceControl] = control;

        var dragHandler = DragDropExtended.GetDragHandler(control);
        dragHandler?.BeforeDragDrop(sender, e, context);
    }

    public void AfterDragDrop(object? sender, PointerEventArgs e, object? context) {
        // Do nothing
    }
}
