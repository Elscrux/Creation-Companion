using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using CreationEditor.Avalonia.Constants;
using Noggog;
namespace CreationEditor.Avalonia.Attached.DragDrop;

public abstract class DragDropHandler<TControl, TElement> : DropHandlerBase, IDragHandler
    where TControl : Control
    where TElement : Control {
    public Orientation Orientation { get; set; }

    protected abstract IEnumerable GetItems(TControl control);
    protected abstract IList GetSelectedItems(TControl control);
    protected abstract int IndexOfElement(TControl control, TElement element);
    protected abstract IEnumerable<TElement> GetElements(TControl control);
    protected abstract TElement? GetLastElement(TControl control);
    protected abstract TElement? GetElementAt(TControl control, int index);
    protected abstract TElement? GetElement(TControl control, object? item);

    public override void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext) {
        base.Over(sender, e, sourceContext, targetContext);

        if (sender is not AvaloniaObject obj) return;
        if (!DragDropExtended.GetAllowDrop(obj)) return;
        if (sourceContext is not DragContext dragContext) return;
        if (!dragContext.Data.TryGetValue(DragDropExtended.DataSourceControl, out var sourceControl)) return;

        if (ReferenceEquals(sender, sourceControl)) {
            // Moving within the same control - apply transforms to preview movement
            ApplyElementTransform(sender, e, dragContext);
        }
    }

    public override void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext) {
        base.Enter(sender, e, sourceContext, targetContext);

        if (sender is not TControl control) return;
        if (!DragDropExtended.GetAllowDrop(control)) return;
        if (sourceContext is not DragContext dragContext) return;

        DragEnterControl(control, e, dragContext);
    }

    public override void Cancel(object? sender, RoutedEventArgs e) {
        if (sender is not TControl control) return;
        if (!DragDropExtended.GetAllowDrop(control)) return;

        AdornerLayer.SetAdorner(control, null);
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) {
        if (sender is not TControl targetControl) return false;

        return DragDropExtended.GetAllowDrop(targetControl);
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state) {
        if (sender is not TControl control) return false;
        if (!DragDropExtended.GetAllowDrop(control)) return false;
        if (sourceContext is not DragContext dragContext) return false;

        Drop(e, dragContext);

        ResetTransform(control);

        return true;
    }

    public void BeforeDragDrop(object? sender, PointerEventArgs e, object? context) {
        if (context is not DragContext dragContext) return;
        if (sender is not Visual visual) return;

        var control = visual.FindAncestorOfType<TControl>(true);
        if (control is null) return;

        dragContext.Data[DragDropExtended.DataStartPosition] = e.GetPosition(control);
        dragContext.Data[DragDropExtended.DataSourceControl] = control;

        var dragHandler = DragDropExtended.GetDragHandler(control);
        dragHandler?.BeforeDragDrop(sender, e, context);
    }

    public void AfterDragDrop(object? sender, PointerEventArgs e, object? context) {
        if (sender is not AvaloniaObject obj) return;

        var dragHandler = DragDropExtended.GetDragHandler(obj);
        dragHandler?.AfterDragDrop(sender, e, context);
    }

    private bool GetData(
        object? sender,
        DragContext dragContext,
        [MaybeNullWhen(false)] out TControl sourceControl,
        [MaybeNullWhen(false)] out IList sourceList,
        [MaybeNullWhen(false)] out TControl targetControl,
        [MaybeNullWhen(false)] out IList targetList) {
        sourceControl = null;
        sourceList = null;
        targetControl = null;
        targetList = null;

        if (sender is not Visual senderVisual) return false;

        // Get target data
        targetControl = senderVisual.FindAncestorOfType<TControl>(true);
        if (targetControl is null) return false;

        // Get source data
        if (!dragContext.Data.TryGetValue(DragDropExtended.DataSourceControl, out var source)) return false;
        if (source is not TControl sourceControlT) return false;

        sourceControl = sourceControlT;

        if (GetItems(sourceControl) is not IList sourceItems) return false;
        if (GetItems(targetControl) is not IList targetItems) return false;

        sourceList = sourceItems;
        targetList = targetItems;

        return true;
    }

    public void DragEnterControl(TControl control, DragEventArgs e, DragContext dragContext) {
        // Get source data
        if (!GetData(
            e.Source,
            dragContext,
            out var sourceControl,
            out var sourceList,
            out var targetControl,
            out var targetList)) return;

        if (ReferenceEquals(sourceControl, targetControl)) return;

        var brush = GetAdornerBrush(sourceList, targetList, sourceControl, targetControl);
        if (brush is null) return;

        var position = e.GetPosition(sourceControl);
        var hoverElement = control.GetVisualsAt(position)
            .OfType<TElement>()
            .FirstOrDefault();

        var index = hoverElement is null
            ? 0
            : IndexOfElement(targetControl, hoverElement);

        dragContext.Data[DragDropExtended.DataCurrentIndex] = index;

        if (hoverElement is null) {
            AdornerLayer.SetAdorner(
                control,
                new Border {
                    BorderBrush = brush,
                    BorderThickness = new Thickness(2),
                    IsHitTestVisible = false,
                });
        } else {
            AdornerLayer.SetAdorner(
                hoverElement,
                new Rectangle {
                    Fill = brush,
                    Height = 2,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    IsHitTestVisible = false,
                });
        }
    }

    public void Drop(DragEventArgs e, DragContext dragContext) {
        // Get source data
        if (!GetData(
            e.Source,
            dragContext,
            out var sourceControl,
            out var sourceList,
            out var targetControl,
            out var targetList)) return;

        if (!dragContext.Data.TryGetValue(DragDropExtended.DataCurrentIndex, out var i) || i is not int index) return;
        if (index == -1 || index > targetList.Count) return;

        var stayInSameList = ReferenceEquals(sourceList, targetList);
        if (!stayInSameList) {
            AdornerLayer.SetAdorner(targetControl, null);
        }

        if (!stayInSameList && !CanDropAny(sourceControl, targetControl)) return;

        var (validItems, invalidItems) = SplitItems(sourceControl, targetControl);

        // Remove source items
        foreach (var selectedItem in validItems) {
            sourceList.Remove(selectedItem);
        }

        // Add target items
        var offset = 0;
        foreach (var selectedItem in validItems) {
            var insertAt = index + offset;
            if (insertAt >= targetList.Count) {
                // If the index is out of bounds, add to the end
                targetList.Add(selectedItem);
            } else {
                targetList.Insert(insertAt, selectedItem);
            }

            offset++;
        }

        // Set new selection
        SetNewSelection(sourceControl, targetControl, validItems, invalidItems);
    }

    private (List<object> ValidItem, List<object> InvalidItems) SplitItems(TControl sourceControl, TControl targetControl) {
        var canDrop = DragDropExtended.GetCanDrop(targetControl);
        var selector = DragDropExtended.GetDropSelector(targetControl);
        var sameControl = ReferenceEquals(sourceControl, targetControl);

        var selectedObjects = GetSelectedItems(sourceControl)
            .OfType<object>()
            .ToArray();

        var validItems = selectedObjects
            .Select(selector)
            .WhereNotNull()
            .Where(obj => sameControl || canDrop(obj))
            .ToList();

        var invalidItems = selectedObjects
            .Except(validItems)
            .ToList();

        return (validItems, invalidItems);
    }

    private bool CanDropAny(TControl sourceControl, TControl targetControl) {
        var canDrop = DragDropExtended.GetCanDrop(targetControl);
        var selector = DragDropExtended.GetDropSelector(targetControl);

        return GetSelectedItems(sourceControl)
            .OfType<object>()
            .Select(selector)
            .WhereNotNull()
            .Any(canDrop);
    }

    private void SetNewSelection(
        TControl sourceControl,
        TControl targetControl,
        List<object> validItems,
        List<object> invalidItems) {
        var sourceSelectedItems = GetSelectedItems(sourceControl);
        sourceSelectedItems.Clear();
        foreach (var invalidItem in invalidItems) {
            sourceSelectedItems.Add(invalidItem);
        }

        var newSelectedItems = GetSelectedItems(targetControl);
        newSelectedItems.Clear();
        foreach (var validItem in validItems) {
            newSelectedItems.Add(validItem);
        }
    }

    private ISolidColorBrush? GetAdornerBrush(IList sourceList, IList targetList, TControl sourceControl, TControl targetControl) {
        return ReferenceEquals(sourceList, targetList) || CanDropAny(sourceControl, targetControl)
            ? StandardBrushes.HighlightBrush
            : StandardBrushes.InvalidBrush;
    }

    private void SetTranslateTransform(Control control, double x, double y) {
        var transformBuilder = new TransformOperations.Builder(1);
        transformBuilder.AppendTranslate(x, y);
        control.RenderTransform = transformBuilder.Build();
    }

    private void ResetTransform(TControl control) {
        foreach (var element in GetElements(control)) {
            SetTranslateTransform(element, 0, 0);
        }
    }

    private void ApplyElementTransform(object? sender, DragEventArgs e, DragContext dragContext) {
        if (!GetData(
            sender,
            dragContext,
            out var sourceControl,
            out var sourceList,
            out var targetControl,
            out _)) return;

        if (!ReferenceEquals(sourceControl, targetControl)) {
            // When dragging out of the control, reset all transforms
            ResetTransform(sourceControl);
            return;
        }

        if (!dragContext.Data.TryGetValue(DragDropExtended.DataStartPosition, out var p) || p is not Point start) return;

        var selected = GetSelectedItems(sourceControl);
        if (selected is not [var firstSelected, ..]) return;

        var selectedElement = GetElement(sourceControl, firstSelected);
        if (selectedElement is null) return;

        var orientation = Orientation;
        var position = e.GetPosition(sourceControl);
        var delta = orientation == Orientation.Horizontal ? position.X - start.X : position.Y - start.Y;

        if (orientation == Orientation.Horizontal) {
            SetTranslateTransform(selectedElement, delta, 0);
        } else {
            SetTranslateTransform(selectedElement, 0, delta);
        }

        var draggedBounds = selectedElement.Bounds;

        var draggedStart = orientation == Orientation.Horizontal ? draggedBounds.X : draggedBounds.Y;

        var draggedDeltaStart = orientation == Orientation.Horizontal
            ? draggedBounds.X + delta
            : draggedBounds.Y + delta;

        var draggedDeltaEnd = orientation == Orientation.Horizontal
            ? draggedBounds.X + delta + draggedBounds.Width
            : draggedBounds.Y + delta + draggedBounds.Height;

        dragContext.Data[DragDropExtended.DataCurrentIndex] = -1;
        var i = 0;
        foreach (var _ in sourceList) {
            var targetContainer = GetElementAt(sourceControl, i);
            if (targetContainer is null) {
                i++;
                continue;
            }

            if (ReferenceEquals(targetContainer, selectedElement)) {
                i++;
                continue;
            }

            var targetBounds = targetContainer.Bounds;

            var targetStart = orientation == Orientation.Horizontal ? targetBounds.X : targetBounds.Y;

            var targetMid = orientation == Orientation.Horizontal
                ? targetBounds.X + targetBounds.Width / 2
                : targetBounds.Y + targetBounds.Height / 2;

            var targetIndex = IndexOfElement(sourceControl, targetContainer);

            if (targetStart > draggedStart && draggedDeltaEnd >= targetMid) {
                if (orientation == Orientation.Horizontal) {
                    SetTranslateTransform(targetContainer, -draggedBounds.Width, 0);
                } else {
                    SetTranslateTransform(targetContainer, 0, -draggedBounds.Height);
                }

                if (dragContext.Data[DragDropExtended.DataCurrentIndex] is not int currentIndex || currentIndex == -1 || currentIndex < targetIndex) {
                    dragContext.Data[DragDropExtended.DataCurrentIndex] = targetIndex;
                }
            } else if (targetStart < draggedStart && draggedDeltaStart <= targetMid) {
                if (orientation == Orientation.Horizontal) {
                    SetTranslateTransform(targetContainer, draggedBounds.Width, 0);
                } else {
                    SetTranslateTransform(targetContainer, 0, draggedBounds.Height);
                }

                if (dragContext.Data[DragDropExtended.DataCurrentIndex] is not int currentIndex || currentIndex == -1 || targetIndex < currentIndex) {
                    dragContext.Data[DragDropExtended.DataCurrentIndex] = targetIndex;
                }
            } else {
                if (orientation == Orientation.Horizontal) {
                    SetTranslateTransform(targetContainer, 0, 0);
                } else {
                    SetTranslateTransform(targetContainer, 0, 0);
                }
            }

            i++;
        }
    }
}
