using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Search;
using DynamicData;
using FluentAvalonia.UI.Controls;
using Noggog;
using Serilog;
namespace CreationEditor.Avalonia.Services.Search;

/// <summary>
/// Provides all assets of the data sources as search items.
/// </summary>
public sealed class AssetSearchSource : ISearchItemSource {
    private readonly IDataSourceService _dataSourceService;
    private readonly IDockFactory _dockFactory;
    private readonly IDockableDefinition? _assetBrowser;
    private readonly ILogger _logger;
    private readonly CompositeDisposable _disposables = [];
    private readonly SourceCache<SearchItem, string> _items = new(x => x.Id);

    public IObservable<IChangeSet<SearchItem, string>> Items => _items.Connect();

    public AssetSearchSource(
        IDataSourceService dataSourceService,
        IDockableRegistry dockableRegistry,
        IDockFactory dockFactory,
        ILogger logger) {
        _dataSourceService = dataSourceService;
        _dockFactory = dockFactory;
        _assetBrowser = dockableRegistry.GetByType(DockableTypes.AssetBrowser);
        _logger = logger;

        dataSourceService.DataSourcesChanged
            .ObserveOnTaskpool()
            .StartWith(dataSourceService.PriorityOrder)
            .Subscribe(_ => UpdateItems())
            .DisposeWith(_disposables);
    }

    private void UpdateItems() {
        _logger.Here().Information("Indexing assets for search");

        var newItems = new List<SearchItem>();
        foreach (var fileLink in _dataSourceService.EnumerateFileLinksInAllDataSources(string.Empty, true)) {
            newItems.Add(CreateItem(fileLink));
        }

        _items.Edit(updater => {
            updater.Clear();
            updater.AddOrUpdate(newItems);
        });
    }

    private SearchItem CreateItem(DataSourceFileLink fileLink) {
        var path = fileLink.DataRelativePath.Path;

        return new SearchItem(
            path,
            path,
            SearchScope.Asset,
            fileLink.DataSource.Name,
            FASymbol.Folder,
            0,
            Execute: () => {
                if (_assetBrowser is null) return Task.CompletedTask;

                Dispatcher.UIThread.Post(() => _dockFactory.OpenFireAndForget(_assetBrowser, parameter: fileLink));
                return Task.CompletedTask;
            },
            ContextFactory: () => new SelectedListContext([], [new AssetContext(fileLink, null)]));
    }

    public void Dispose() {
        _disposables.Dispose();
        _items.Dispose();
    }
}
