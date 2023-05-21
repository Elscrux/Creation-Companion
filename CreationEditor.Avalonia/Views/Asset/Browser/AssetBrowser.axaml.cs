﻿using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Input;
using Avalonia.ReactiveUI;
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

    private void TreeDataGrid_OnRowDragStarted(object? sender, TreeDataGridRowDragStartedEventArgs e) {
        foreach (var asset in e.Models.OfType<AssetTreeItem>()) {
            if (asset.IsVirtual) {
                e.AllowedEffects = DragDropEffects.None;
            }
        }
    }

    private void TreeDataGrid_OnRowDragOver(object? sender, TreeDataGridRowDragEventArgs e) {
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

    private void TreeDataGrid_OnRowDrop(object? sender, TreeDataGridRowDragEventArgs e) {
        // Never allow the tree to drop things, changes are handled by file system watchers
        e.Inner.DragEffects = DragDropEffects.None;

        ViewModel?.Drop(e);
    }
}
