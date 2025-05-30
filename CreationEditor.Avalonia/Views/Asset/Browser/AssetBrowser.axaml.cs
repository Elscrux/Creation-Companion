using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Services.DataSource;
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
        if (e.Source is not Control { DataContext: FileSystemLink asset } control) return;

        var contextFlyout = new MenuFlyout {
            ItemsSource = ViewModel?.GetContextMenuItems(asset),
        };

        contextFlyout.ShowAt(control, true);

        e.Handled = true;
    }

    private void AssetTree_OnRowDragStarted(object? sender, TreeDataGridRowDragStartedEventArgs e) {
        e.AllowedEffects = e.Models.ToArray().OfType<FileSystemLink>().Any(x => x.DataSource.IsReadOnly)
            ? DragDropEffects.None
            : DragDropEffects.Move;
    }

    private void AssetTree_OnRowDragOver(object? sender, TreeDataGridRowDragEventArgs e) {
        e.Inner.DragEffects = DragDropEffects.Move;

        if (e.TargetRow.Model is FileSystemLink { IsDirectory: true } directoryLink
         && e.Position is not (TreeDataGridRowDropPosition.After or TreeDataGridRowDropPosition.Before)) {
            if (e.Inner.Data.Get(DragInfo.DataFormat) is DragInfo dragInfo) {
                var assets = dragInfo.Indexes
                    .Select(indexPath => {
                        var rowIndex = dragInfo.Source.Rows.ModelIndexToRowIndex(indexPath);
                        var row = dragInfo.Source.Rows[rowIndex];
                        return row.Model;
                    })
                    .OfType<FileSystemLink>();

                if (assets.Any(asset => asset == directoryLink || asset.ParentDirectory == directoryLink)) {
                    e.Inner.DragEffects = DragDropEffects.None;
                }
            }
        } else {
            e.Inner.DragEffects = DragDropEffects.None;
        }
    }

    private void AssetTree_OnRowDrop(object? sender, TreeDataGridRowDragEventArgs e) {
        // Never allow the tree to drop things, changes are handled by file system watchers
        e.Inner.DragEffects = DragDropEffects.None;

        if (e.TargetRow.Model is not FileSystemLink { IsDirectory: true } directoryLink) return;
        if (e.Inner.Data.Get(DragInfo.DataFormat) is not DragInfo dragInfo) return;

        ViewModel?.Drop(directoryLink, dragInfo);
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
                if (AssetTree.RowSelection.SelectedItem is FileSystemLink item) {
                    if (!await ViewModel.OpenReferences.CanExecute) break;

                    await ViewModel.OpenReferences.Execute(item);
                }
                break;
            // Rename
            case Key.F2:
                if (AssetTree.RowSelection.SelectedItem is FileSystemLink fileSystemLink) {
                    if (!await ViewModel.Rename.CanExecute) break;

                    await ViewModel.Rename.Execute(fileSystemLink);
                }
                break;
            // Delete
            case Key.Delete:
                await ViewModel.Delete.Execute(AssetTree.RowSelection.SelectedItems.OfType<FileSystemLink?>().ToList());
                break;
        }
    }

    private async void AssetTree_OnDoubleTapped(object? sender, TappedEventArgs e) {
        if (AssetTree.RowSelection is null || ViewModel is null) return;

        // Ignore double taps on the chevron
        if (e.Source is Visual v && v.GetVisualChildren().Any(x => x.Name == "ChevronPath")) return;

        if (AssetTree.RowSelection.SelectedItem is FileSystemLink { IsFile: true }) {
            await ViewModel.Open.Execute(AssetTree.RowSelection.SelectedItems.OfType<FileSystemLink>().ToList());
        } else {
            if (e.Source is not Visual visual) return;

            var expanderCell = visual.FindAncestorOfType<TreeDataGridExpanderCell>();
            if (expanderCell is not null) {
                expanderCell.IsExpanded = !expanderCell.IsExpanded;
            }
        }
    }
}
