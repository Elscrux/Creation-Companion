using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using LeveledList.Model.List;
using LeveledList.ViewModels;
using ReactiveUI;

namespace LeveledList.Views;

public partial class ListsView : ReactiveUserControl<ListsVM> {
    public ListsView() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(x => x.ListsDataGrid.SelectedItem)
                .Subscribe(_ => {
                    if (ViewModel is null) return;

                    var selectedDefinitions = ListsDataGrid.SelectedItems
                        .OfType<ExtendedListDefinition>()
                        .ToArray();

                    ViewModel.SelectedLists = selectedDefinitions;
                })
                .DisposeWith(disposables);
        });
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
