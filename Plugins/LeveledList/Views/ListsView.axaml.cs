using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using LeveledList.Model.List;
using LeveledList.ViewModels;

namespace LeveledList.Views;

public partial class ListsView : ReactiveUserControl<ListsVM> {
    public ListsView() {
        InitializeComponent();
    }

    private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e) {
        ViewModel?.ContextMenu(sender, e);
    }

    private void OpenFile(object? sender, RoutedEventArgs e) {
        if (ListsDataGrid.SelectedItem is not ExtendedListDefinition listDefinition) return;

        Process.Start(new ProcessStartInfo {
            FileName = listDefinition.Path,
            UseShellExecute = true,
            Verb = "open",
        });
    }
}
