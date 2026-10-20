using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Search;
using DynamicData;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace CreationEditor.Avalonia.Services.Search;

/// <summary>
/// Provides all records of the current mod scope as search items.
/// Records are indexed in the background; placed record types are excluded.
/// </summary>
public sealed partial class RecordSearchSource : ReactiveObject, ISearchItemSource {
    private readonly IMutagenTypeProvider _mutagenTypeProvider;
    private readonly IMutagenCommonAspectsProvider _mutagenCommonAspectsProvider;
    private readonly IGenericContextActionsProvider _genericContextActionsProvider;
    private readonly IReferenceService _referenceService;
    private readonly ILogger _logger;
    private readonly CompositeDisposable _disposables = [];

    [Reactive] public partial bool IsBusy { get; set; }

    private readonly SourceCache<SearchItem, string> _items = new(x => x.Id);
    public IObservable<IChangeSet<SearchItem, string>> Items => _items.Connect();

    public RecordSearchSource(
        IModScopeProvider modScopeProvider,
        IMutagenTypeProvider mutagenTypeProvider,
        IMutagenCommonAspectsProvider mutagenCommonAspectsProvider,
        IGenericContextActionsProvider genericContextActionsProvider,
        IReferenceService referenceService,
        ILogger logger) {
        _mutagenTypeProvider = mutagenTypeProvider;
        _mutagenCommonAspectsProvider = mutagenCommonAspectsProvider;
        _genericContextActionsProvider = genericContextActionsProvider;
        _referenceService = referenceService;
        _logger = logger;

        modScopeProvider.LinkCacheChanged
            .ObserveOnTaskpool()
            .WrapInProgressMarker(x => x.Do(UpdateItems), out var isBusy)
            .Subscribe()
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.IsBusy)
            .Subscribe()
            .DisposeWith(_disposables);
        isBusy.Subscribe(x => Dispatcher.UIThread.Post(() => IsBusy = x)).DisposeWith(_disposables);
    }

    private void UpdateItems(ILinkCache linkCache) {
        _logger.Here().Information("Indexing records for search");

        var newItems = new List<SearchItem>();
        foreach (var mod in linkCache.ListedOrder) {
            foreach (var record in mod.EnumerateMajorRecords()) {
                // Placed records are game specific (IPlacedGetter lives in the game assemblies),
                // so exclude them by interface name here
                if (record.Type.GetInterfaces().Any(i => i.InheritsFrom(_mutagenCommonAspectsProvider.PlacedType))) continue;

                newItems.Add(CreateItem(record, linkCache));
            }
        }

        _items.Edit(updater => {
            updater.Clear();
            updater.AddOrUpdate(newItems);
        });
    }

    private SearchItem CreateItem(IMajorRecordGetter record, ILinkCache linkCache) {
        var typeName = _mutagenTypeProvider.GetTypeName(record.Type);
        var title = string.IsNullOrEmpty(record.EditorID) ? record.FormKey.ToString() : record.EditorID;
        var formKey = record.FormKey;

        return new SearchItem(
            formKey.ToString(),
            title,
            SearchScope.Record,
            $"{formKey} · {typeName}",
            FASymbol.BulletList,
            0,
            [typeName],
            () => {
                Dispatcher.UIThread.Post(() => _genericContextActionsProvider.EditRecord(record));
                return Task.CompletedTask;
            },
            ContextFactory: () => CreateContext(record, linkCache));
    }

    private SelectedListContext CreateContext(IMajorRecordGetter record, ILinkCache linkCache) {
        ModKey? origin = null;
        try {
            origin = linkCache.GetWinningOverrideModKey(record);
        } catch {
            // Record might not be in the cache anymore
        }

        _referenceService.GetReferencedRecord(record, out var referencedRecord);

        return new SelectedListContext(
            [new RecordContext(origin, referencedRecord)],
            []);
    }

    public void Dispose() {
        _disposables.Dispose();
        _items.Dispose();
    }
}
