using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Behavior;
using ReleasePipeline.Models;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Behavior;

/// <summary>
/// Starts a drag operation when the associated drag handle is pressed and moved beyond the
/// drag threshold. The dragged payload is the action's <see cref="PipelineActionVM"/>.
/// </summary>
public sealed class DragHandleBehavior : Behavior<Control> {
    private readonly DragStartHandler _dragStartHandler = new(Drag);

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null) return;
        _dragStartHandler.Register(AssociatedObject);

        // Prevent the Expander header ToggleButton from capturing the pointer, so the drag
        // handle keeps receiving pointer-move events to start the drag.
        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Direct);
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null) return;
        _dragStartHandler.Unregister(AssociatedObject);
        AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
        e.Handled = true;
    }

    private static async Task Drag(object? sender, object? identifier, PointerEventArgs e) {
        if (sender is not Control { DataContext: PipelineActionVM actionVM } control) return;

        var dataTransfer = IDataTransfer.Create(new PipelineActionDragData { Action = actionVM });
        var position = e.GetCurrentPoint(control).Position;
        var args = new PointerPressedEventArgs(
            e.Source,
            e.Pointer,
            control,
            position,
            e.Timestamp,
            e.GetCurrentPoint(control).Properties,
            e.KeyModifiers);

        await DragDrop.DoDragDropAsync(args, dataTransfer, DragDropEffects.Move);
    }
}
