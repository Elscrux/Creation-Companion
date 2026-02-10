using System.Diagnostics;
using System.Reactive.Disposables;
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
            ListsDataGrid.SelectedItems.Clear();
            if (ViewModel?.SelectedDefinitions is not null) {
                foreach (var definition in ViewModel.SelectedDefinitions) {
                    ListsDataGrid.SelectedItems.Add(definition);
                }
            }

            this.WhenAnyValue(x => x.ListsDataGrid.SelectedItem)
                .Subscribe(_ => {
                    if (ViewModel is null) return;

                    var selectedDefinitions = ListsDataGrid.SelectedItems
                        .OfType<ExtendedListDefinition>()
                        .ToArray();

                    // Only update if the selected definitions have changed to avoid unnecessary updates
                    if (ViewModel.SelectedDefinitions is not null && selectedDefinitions.SequenceEqual(ViewModel.SelectedDefinitions)) return;

                    ViewModel.SelectedDefinitions = selectedDefinitions;
                })
                .DisposeWith(disposables);
        });
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
