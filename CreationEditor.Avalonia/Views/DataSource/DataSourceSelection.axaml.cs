using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Models.DataSource;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Services.DataSource;
using Noggog;
namespace CreationEditor.Avalonia.Views.DataSource;

public partial class DataSourceSelection : ReactiveUserControl<IDataSourceSelectionVM> {
    public DataSourceSelection() {
        InitializeComponent();
    }

    private static void TreeDataGrid_OnRowDragStarted(object? sender, TreeDataGridRowDragStartedEventArgs e) {
        e.AllowedEffects = e.Models.OfType<DataSourceItem>().Any(x => x.DataSource is ArchiveDataSource)
            ? DragDropEffects.None
            : DragDropEffects.Move;
    }

    private static void TreeDataGrid_OnRowDragOver(object? sender, TreeDataGridRowDragEventArgs e) {
        e.Inner.DragEffects = DragDropEffects.Move;

        if (e.TargetRow.Model is not DataSourceItem
         || e.Position is not (TreeDataGridRowDropPosition.After or TreeDataGridRowDropPosition.Before)) {
            e.Inner.DragEffects = DragDropEffects.None;
        }
    }

    private void TreeDataGrid_OnRowDrop(object? sender, TreeDataGridRowDragEventArgs e) {
        if (ViewModel is null) return;

        // Never allow the tree to drop things, changes are handled by file system watchers
        e.Inner.DragEffects = DragDropEffects.None;

        if (e.TargetRow.Model is not DataSourceItem item) return;
        if (e.Inner.Data.Get(DragInfo.DataFormat) is not DragInfo dragInfo) return;

        Console.WriteLine();

        var indexOf = ViewModel.DataSources.IndexOf(item);

        var targetIndex = e.Position switch {
            TreeDataGridRowDropPosition.After => indexOf + 1,
            _ => indexOf
        };

        var items = dragInfo.Indexes
            .Select(indexPath => {
                var rowIndex = dragInfo.Source.Rows.ModelIndexToRowIndex(indexPath);
                var row = dragInfo.Source.Rows[rowIndex];
                return row.Model;
            })
            .OfType<DataSourceItem>()
            .ToArray();

        if (dragInfo.Source.Items is IList<DataSourceItem> l) {
            l.Remove(items);
        }
        ViewModel.DataSources.InsertRange(items, targetIndex);
    }

    private async void InputElement_OnKeyUp(object? sender, KeyEventArgs e) {
        if (ViewModel is null) return;

        switch (e.Key) {
            case Key.Delete:
                if (DataSourceTree.RowSelection is null) return;

                var itemsToRemove = DataSourceTree.RowSelection.SelectedItems
                    .OfType<DataSourceItem>()
                    .Where(x => ViewModel.CanRemoveDataSource(x))
                    .ToList();

                if (itemsToRemove.Count == 0) return;

                await ViewModel.RemoveDataSource.Execute(itemsToRemove);

                break;
            case Key.Space: {
                if (DataSourceTree.RowSelection is null) return;

                foreach (var item in DataSourceTree.RowSelection.SelectedItems.OfType<DataSourceItem>()) {
                    item.IsSelected = !item.IsSelected;
                }
                break;
            }
        }
    }
}
