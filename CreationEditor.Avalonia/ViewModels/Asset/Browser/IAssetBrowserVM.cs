using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
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

    ReactiveCommand<IReadOnlyList<FileSystemLink?>, Unit> Open { get; }
    ReactiveCommand<IReadOnlyList<FileSystemLink?>, Unit> Delete { get; }
    ReactiveCommand<FileSystemLink, Unit> Rename { get; }
    ReactiveCommand<FileSystemLink, Unit> OpenReferences { get; }
    ReactiveCommand<DataRelativePath, Unit> MoveTo { get; }
    ReactiveCommand<FileSystemLink, Unit> AddFolder { get; }
    ReactiveCommand<FileSystemLink, Unit> OpenAssetBrowser { get; }

    bool IsBusyLoadingAssets { get; set; }
    bool IsBusyLoadingReferences { get; set; }

    HierarchicalTreeDataGridSource<FileSystemLink> AssetTreeSource { get; }

    Task Drop(FileSystemLink dstDirectory, DragInfo dragInfo);
    IEnumerable<Control> GetContextMenuItems(FileSystemLink asset);
}
