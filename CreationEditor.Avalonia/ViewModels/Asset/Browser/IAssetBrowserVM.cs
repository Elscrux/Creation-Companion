using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
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

    ReactiveCommand<IReadOnlyList<IDataSourceLink?>, Unit> Open { get; }
    ReactiveCommand<IReadOnlyList<IDataSourceLink?>, Unit> Delete { get; }
    ReactiveCommand<IDataSourceLink, Unit> Rename { get; }
    ReactiveCommand<DataSourceFileLink, Unit> OpenReferences { get; }
    ReactiveCommand<DataRelativePath, Unit> MoveTo { get; }
    ReactiveCommand<DataSourceDirectoryLink, Unit> AddFolder { get; }

    bool IsBusyLoadingAssets { get; set; }
    bool IsBusyLoadingReferences { get; set; }

    HierarchicalTreeDataGridSource<IDataSourceLink> AssetTreeSource { get; }
    IDataSource DataSource { get; set; }
    ReadOnlyObservableCollection<IDataSource> DataSourceSelections { get; }

    IAssetTypeService AssetTypeService { get; }
    IAssetIconService AssetIconService { get; }

    IAssetLinkGetter? GetAssetLink(IDataSourceLink fileLink);
    Task Drop(DataSourceDirectoryLink dstDirectory, DragInfo dragInfo);
    IEnumerable<Control> GetContextMenuItems(IDataSourceLink asset);
}
