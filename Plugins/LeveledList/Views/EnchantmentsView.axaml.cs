using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using LeveledList.Model.Enchantments;
using LeveledList.Model.List;
using LeveledList.ViewModels;
using ReactiveUI;

namespace LeveledList.Views;

public partial class EnchantmentsView : ReactiveUserControl<EnchantmentsVM> {
    public EnchantmentsView() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(x => x.ListsDataGrid.SelectedItem)
                .Subscribe(_ => {
                    if (ViewModel is null) return;

                    var selectedDefinitions = ListsDataGrid.SelectedItems
                        .OfType<ExtendedEnchantmentItem>()
                        .ToArray();

                    ViewModel.SelectedEnchantmentItems = selectedDefinitions;
                })
                .DisposeWith(disposables);
        });
    }

    private void OpenFile(object? sender, RoutedEventArgs e) {
        if (ListsDataGrid.SelectedItem is not ExtendedEnchantmentItem listDefinition) return;

        Process.Start(new ProcessStartInfo {
            FileName = listDefinition.Path,
            UseShellExecute = true,
            Verb = "open",
        });
    }
}
