using Avalonia.Controls;
using Avalonia.ReactiveUI;
using LeveledList.ViewModels;

namespace LeveledList.Views;

public partial class ListsView : ReactiveUserControl<ListsVM> {
    public ListsView() {
        InitializeComponent();
    }

    private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e) {
        ViewModel?.ContextMenu(sender, e);
    }
}

