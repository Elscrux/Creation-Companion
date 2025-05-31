using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Asset.Browser;

public interface IAssetBrowserVM : IDisposableDropoff {
    string SearchText { get; set; }
    bool ShowEmptyDirectories { get; set; }
    bool ShowIgnoredDirectories { get; set; }
    bool ShowReferencedFiles { get; set; }
    bool ShowOrphanedFiles { get; set; }
    bool ShowOtherFiles { get; set; }
    bool ShowTextures { get; set; }
    bool ShowModels { get; set; }
    bool ShowScriptSources { get; set; }
    bool ShowScripts { get; set; }
    bool ShowSounds { get; set; }
    bool ShowMusic { get; set; }
    bool ShowBehaviors { get; set; }
    bool ShowBodyTextures { get; set; }
    bool ShowDeformedModels { get; set; }
    bool ShowInterfaces { get; set; }
    bool ShowSeq { get; set; }
    bool ShowTranslations { get; set; }

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
    IDataSource DataSource { get; set; }
    ReadOnlyObservableCollection<IDataSource> DataSourceSelections { get; }

    IAssetTypeService AssetTypeService { get; }
    IAssetIconService AssetIconService { get; }

    Task Drop(FileSystemLink dstDirectory, DragInfo dragInfo);
    IEnumerable<Control> GetContextMenuItems(FileSystemLink asset);
}
