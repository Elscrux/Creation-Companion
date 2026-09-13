using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Behavior;
using ReactiveUI.Avalonia;
using ReleasePipeline.Models;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class ReleasePipelineEditorView : ReactiveUserControl<ReleasePipelineEditorVM> {
    public ReleasePipelineEditorView() {
        InitializeComponent();

        ActionsListBox.AddHandler(DragDrop.DragOverEvent, OnDragOver);
        ActionsListBox.AddHandler(DragDrop.DropEvent, OnDrop);
        DragDrop.SetAllowDrop(ActionsListBox, true);
    }

    public ReleasePipelineEditorView(ReleasePipelineEditorVM vm) : this() {
        DataContext = vm;
    }

    private void OnDragOver(object? sender, DragEventArgs e) {
        if (!e.DataTransfer.TryGet<PipelineActionDragData>(out _)) return;

        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e) {
        if (!e.DataTransfer.TryGet<PipelineActionDragData>(out var dragData)) return;
        if (dragData.Action is null) return;
        if (ViewModel is not { } vm) return;

        var sourceIndex = vm.ActionVMs.IndexOf(dragData.Action);
        if (sourceIndex < 0) return;

        // Determine the target index from the item under the pointer.
        var targetIndex = GetTargetIndex(e);
        if (targetIndex < 0) return;

        // Adjust for removal of the source item when it precedes the target.
        if (sourceIndex < targetIndex) targetIndex--;

        vm.ActionVMs.Move(sourceIndex, targetIndex);
        e.Handled = true;
    }

    private int GetTargetIndex(DragEventArgs e) {
        var position = e.GetPosition(ActionsListBox);
        var item = ActionsListBox.GetVisualsAt(position)
            .OfType<ListBoxItem>()
            .FirstOrDefault();

        if (item is null) return -1;

        return ActionsListBox.IndexFromContainer(item);
    }
}
