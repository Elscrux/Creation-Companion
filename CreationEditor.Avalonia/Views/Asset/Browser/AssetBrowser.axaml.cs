using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Asset.Browser;

public partial class AssetBrowser : ReactiveUserControl<IAssetBrowserVM> {
    public AssetBrowser() {
        InitializeComponent();
    }

    public AssetBrowser(IAssetBrowserVM assetBrowserVM) : this() {
        ViewModel = assetBrowserVM;
        this.WhenActivated(d => {
            AssetTree.ContextRequested += ContextRequestHandler;
            d.Add(Disposable.Create(() => AssetTree.ContextRequested -= ContextRequestHandler));

        });
    }

    private void ContextRequestHandler(object? sender, ContextRequestedEventArgs e) {
        if (e.Source is not Control { DataContext: AssetTreeItem asset } control) return;

        var contextFlyout = new MenuFlyout {
            ItemsSource = ViewModel?.GetContextMenuItems(asset),
        };

        contextFlyout.ShowAt(control, true);

        e.Handled = true;
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

        if (e.TargetRow.Model is not AssetTreeItem { Asset: AssetDirectory directory }) return;
        if (e.Inner.Data.Get(DragInfo.DataFormat) is not DragInfo dragInfo) return;

        ViewModel?.Drop(directory, dragInfo);
    }

    private async void AssetTree_OnKeyDown(object? sender, KeyEventArgs e) {
        if (AssetTree.RowSelection is null || ViewModel is null) return;

        switch (e.Key) {
            // Focus search box
            case Key.F when (e.KeyModifiers & KeyModifiers.Control) != 0:
                SearchBox.Focus();
                break;
            // Open references
            case Key.R when (e.KeyModifiers & KeyModifiers.Control) != 0:
                if (AssetTree.RowSelection.SelectedItem is AssetTreeItem item) {
                    if (!await ViewModel.OpenReferences.CanExecute) break;

                    await ViewModel.OpenReferences.Execute(item);
                }
                break;
            // Rename
            case Key.F2:
                if (AssetTree.RowSelection.SelectedItem is AssetTreeItem assetTreeItem) {
                    if (!await ViewModel.Rename.CanExecute) break;

                    await ViewModel.Rename.Execute(assetTreeItem);
                }
                break;
            // Delete
            case Key.Delete:
                await ViewModel.Delete.Execute(AssetTree.RowSelection.SelectedItems.OfType<AssetTreeItem?>().ToList());
                break;
        }
    }

    private async void AssetTree_OnDoubleTapped(object? sender, TappedEventArgs e) {
        if (AssetTree.RowSelection is null || ViewModel is null) return;

        // Ignore double taps on the chevron
        if (e.Source is Visual v && v.GetVisualChildren().Any(x => x.Name == "ChevronPath")) return;

        if (AssetTree.RowSelection.SelectedItem is AssetTreeItem { IsDirectory: true }) {
            if (e.Source is not Visual visual) return;

            var expanderCell = visual.FindAncestorOfType<TreeDataGridExpanderCell>();
            if (expanderCell is not null) {
                expanderCell.IsExpanded = !expanderCell.IsExpanded;
            }
        } else {
            await ViewModel.Open.Execute(AssetTree.RowSelection.SelectedItems.OfType<AssetTreeItem>().ToList());
        }
    }
}
