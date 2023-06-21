using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DockDragBehavior : Behavior<Control> {
    private readonly DragHandler _dragHandler;

    public DockDragBehavior() {
        _dragHandler = new DragHandler(Drag);
    }

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject is null) return;

        _dragHandler.Register(AssociatedObject);
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        if (AssociatedObject is null) return;

        _dragHandler.Unregister(AssociatedObject);
    }

    private async void Drag(object? sender, object? identifier, PointerEventArgs e) {
        if (sender is not Control { DataContext: IDockedItem dockedItem }) return;

        var dataObject = new DataObject();
        dataObject.Set(nameof(DockDragData), new DockDragData { Item = dockedItem });

        SetDragging(true);
        var result = await DragDrop.DoDragDrop(e, dataObject, DragDropEffects.Move);
        SetDragging(false);

        if (result == DragDropEffects.None) {
            var data = dataObject.Get(nameof(DockDragData));
            if (data is DockDragData dragData) {
                dragData.Preview?.HidePreview();
            }
        }

        void SetDragging(bool state) {
            dockedItem.DockRoot.SetEditMode(state);
        }
    }
}
