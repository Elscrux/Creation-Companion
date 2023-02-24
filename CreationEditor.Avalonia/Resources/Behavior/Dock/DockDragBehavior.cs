using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DockDragBehavior : Behavior<Control> {
    private bool _isPressingDown;
    private bool _isDragging;

    protected override void OnAttached() {
        base.OnAttached();

        if (AssociatedObject == null) return;

        AssociatedObject.AddHandler(InputElement.PointerMovedEvent, (sender, e) => {
            if (!_isPressingDown || _isDragging) return;

            if (sender is Control { DataContext: IDockedItem dockedItem }) {
                Drag(dockedItem, e);
            }
        });
        
        AssociatedObject.AddHandler(InputElement.PointerPressedEvent, (_, _) => _isPressingDown = true);
        AssociatedObject.AddHandler(InputElement.PointerExitedEvent, (_, _) => _isPressingDown = false);
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, (_, _) => _isPressingDown = false);
    }

    private async void Drag(IDockedItem dockedItem, PointerEventArgs e) {
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

            _isDragging = state;
        }
    }
}
