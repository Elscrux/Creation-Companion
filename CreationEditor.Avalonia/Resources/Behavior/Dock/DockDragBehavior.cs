using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Models.Docking;
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
        if (sender is not Control { DataContext: IDockedItem dockedItem }) return;

        var dataTransfer = IDataTransfer.Create(new DockDragData { Item = dockedItem });

        SetDragging(true);
        var result = await DragDrop.DoDragDropAsync(e, dataTransfer, DragDropEffects.Move);
        SetDragging(false);

        if (result == DragDropEffects.None && dataTransfer.TryGet<DockDragData>(out var data)) { 
            data.Preview?.HidePreview();
        }

        void SetDragging(bool state) {
            dockedItem.DockRoot.SetEditMode(state);
        }
    }
}
