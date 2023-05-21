using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Asset;
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

    bool IsBusyLoadingAssets { get; set; }
    bool IsBusyLoadingReferences { get; set; }

    HierarchicalTreeDataGridSource<AssetTreeItem> AssetTreeSource { get; }

    Task Drop(TreeDataGridRowDragEventArgs treeDataGridRowDragEventArgs);
    void ContextMenu(object? sender, ContextRequestedEventArgs e);
}
