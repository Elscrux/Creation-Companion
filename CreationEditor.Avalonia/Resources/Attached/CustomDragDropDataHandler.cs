using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;
using CreationEditor.Avalonia.Constants;
namespace CreationEditor.Avalonia.Attached;

public sealed class CustomDragDropDataHandler<T, TData> : DropHandlerBase, IDragHandler
    where T : ICustomDragDropData<TData> {
    public void BeforeDragDrop(object? sender, PointerEventArgs e, object? context) {
        if (context is not DragContext dragContext) return;
        if (!dragContext.Data.TryGetValue(DragDropExtended.DataSourceControl, out var source)) return;

        if (sender is not StyledElement senderElement) return;
        if (source is not AvaloniaObject sourceControl) return;

        var getData = T.GetData(sourceControl);
        if (getData is null) return;

        var data = getData(senderElement.DataContext);

        dragContext.Data[T.Data] = data;
    }

    public void AfterDragDrop(object? sender, PointerEventArgs e, object? context) {}

    public override void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext) {
        base.Enter(sender, e, sourceContext, targetContext);

        if (sourceContext is not DragContext dragContext) return;
        if (!dragContext.Data.TryGetValue(T.Data, out var link)) return;
        if (link is not TData assetLink) return;
        if (sender is not Visual visual) return;

        var setData = T.GetSetData(visual);
        if (setData is null) return;

        var canSetData = T.GetCanSetData(visual);

        // Show adorner when target has setter for form link
        AdornerLayer.SetAdorner(
            visual,
            new Border {
                BorderBrush = canSetData is not null && canSetData(assetLink)
                    ? StandardBrushes.HighlightBrush
                    : StandardBrushes.InvalidBrush,
                BorderThickness = new Thickness(2),
                IsHitTestVisible = false,
            });
    }

    public override void Leave(object? sender, RoutedEventArgs e) {
        base.Leave(sender, e);

        if (sender is not Visual visual) return;

        var setData = T.GetSetData(visual);
        if (setData is null) return;

        // Hide adorner when target has setter for form link
        AdornerLayer.SetAdorner(visual, null);
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) {
        if (sourceContext is not DragContext dragContext) return false;
        if (!dragContext.Data.TryGetValue(T.Data, out var link)) return false;
        if (link is not TData data) return false;
        if (sender is not Visual visual) return false;

        var canSetData = T.GetCanSetData(visual);
        return canSetData is null || canSetData(data);
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) {
        if (sourceContext is not DragContext dragContext) return false;
        if (!dragContext.Data.TryGetValue(T.Data, out var link)) return false;
        if (link is not TData data) return false;
        if (sender is not Visual visual) return false;

        AdornerLayer.SetAdorner(visual, null);

        var canSetData = T.GetCanSetData(visual);
        if (canSetData is not null && !canSetData(data)) return false;

        var setData = T.GetSetData(visual);
        if (setData is null) return false;

        setData(data);

        return true;
    }
}
