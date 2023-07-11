using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Models.Asset;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Services.Asset;
namespace CreationEditor.Avalonia.Views.Asset.Browser;

public partial class AssetBrowser : ReactiveUserControl<IAssetBrowserVM> {
    public AssetBrowser() {
        InitializeComponent();
    }

    public AssetBrowser(IAssetBrowserVM assetBrowserVM) : this() {
        ViewModel = assetBrowserVM;
        AssetTree.ContextRequested += ViewModel.ContextMenu;
    }

    private void AssetTree_OnRowDragStarted(object? sender, TreeDataGridRowDragStartedEventArgs e) {
        foreach (var asset in e.Models.OfType<AssetTreeItem>()) {
            if (asset.IsVirtual) {
                e.AllowedEffects = DragDropEffects.None;
            }
        }
    }

    private void AssetTree_OnRowDragOver(object? sender, TreeDataGridRowDragEventArgs e) {
        if (e.TargetRow.Model is not AssetTreeItem { IsDirectory: true } assetTreeItem
         || e.Position is TreeDataGridRowDropPosition.After or TreeDataGridRowDropPosition.Before) {
            e.Inner.DragEffects = DragDropEffects.None;
        } else if (e.Inner.Data.Get(DragInfo.DataFormat) is DragInfo dragInfo) {
            var assets = dragInfo.Indexes
                .Select(indexPath => {
                    var rowIndex = dragInfo.Source.Rows.ModelIndexToRowIndex(indexPath);
                    var row = dragInfo.Source.Rows[rowIndex];
                    return row.Model;
                })
                .OfType<AssetTreeItem>();

            if (assets.Any(asset => assetTreeItem.Path.StartsWith(asset.Path, AssetCompare.PathComparison))) {
                e.Inner.DragEffects = DragDropEffects.None;
            }
        }
    }

    private void AssetTree_OnRowDrop(object? sender, TreeDataGridRowDragEventArgs e) {
        // Never allow the tree to drop things, changes are handled by file system watchers
        e.Inner.DragEffects = DragDropEffects.None;

        ViewModel?.Drop(e);
    }

    private void AssetTree_OnKeyDown(object? sender, KeyEventArgs e) {
        if (AssetTree.RowSelection is null || ViewModel is null) return;

        switch (e.Key) {
            // Focus search box
            case Key.F when (e.KeyModifiers & KeyModifiers.Control) != 0:
                SearchBox.Focus();
                break;
            // Open references
            case Key.R when (e.KeyModifiers & KeyModifiers.Control) != 0:
                if (AssetTree.RowSelection.SelectedItem is AssetTreeItem item) {
                    var command = ViewModel.OpenReferences as ICommand;
                    if (command.CanExecute(null)) {
                        command.Execute(item);
                    }
                }
                break;
            // Rename
            case Key.F2:
                if (AssetTree.RowSelection.SelectedItem is AssetTreeItem assetTreeItem) {
                    var command = ViewModel.Rename as ICommand;
                    if (command.CanExecute(null)) {
                        command.Execute(assetTreeItem);
                    }
                }
                break;
            // Delete
            case Key.Delete:
                (ViewModel.Delete as ICommand).Execute(AssetTree.RowSelection.SelectedItems.OfType<AssetTreeItem?>().ToList());
                break;
        }
    }

    private void AssetTree_OnDoubleTapped(object? sender, TappedEventArgs e) {
        if (AssetTree.RowSelection is null || ViewModel is null) return;

        if (AssetTree.RowSelection.SelectedItem is AssetTreeItem { IsDirectory: true }) {
            if (e.Source is not Visual visual) return;

            var expanderCell = visual.FindAncestorOfType<TreeDataGridExpanderCell>();
            if (expanderCell is not null) {
                expanderCell.IsExpanded = !expanderCell.IsExpanded;
            }
        } else {
            (ViewModel.Open as ICommand).Execute(AssetTree.RowSelection.SelectedItems.OfType<AssetTreeItem>().ToList());
        }
    }
}
