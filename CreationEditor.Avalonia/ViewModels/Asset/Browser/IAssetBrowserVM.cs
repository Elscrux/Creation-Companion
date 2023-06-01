using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Asset;
using CreationEditor.Services.Asset;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Asset.Browser;

public interface IAssetBrowserVM : IDisposableDropoff {
    string SearchText { get; set; }
    bool ShowBsaAssets { get; set; }
    bool ShowEmptyDirectories { get; set; }
    bool ShowOnlyOrphaned { get; set; }

    ReactiveCommand<Unit, Unit> Undo { get; }
    ReactiveCommand<Unit, Unit> Redo { get; }

    ReactiveCommand<IReadOnlyList<AssetTreeItem?>, Unit> Open { get; }
    ReactiveCommand<IReadOnlyList<AssetTreeItem?>, Unit> Delete { get; }
    ReactiveCommand<AssetTreeItem, Unit> Rename { get; }
    ReactiveCommand<AssetTreeItem, Unit> OpenReferences { get; }
    ReactiveCommand<AssetDirectory, Unit> AddFolder { get; }
    ReactiveCommand<AssetDirectory, Unit> OpenAssetBrowser { get; }

    bool IsBusyLoadingAssets { get; set; }
    bool IsBusyLoadingReferences { get; set; }

    HierarchicalTreeDataGridSource<AssetTreeItem> AssetTreeSource { get; }

    Task Drop(TreeDataGridRowDragEventArgs treeDataGridRowDragEventArgs);
    void ContextMenu(object? sender, ContextRequestedEventArgs e);
}
