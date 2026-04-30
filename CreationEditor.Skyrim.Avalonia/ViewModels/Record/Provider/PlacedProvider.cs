using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed partial class PlacedProvider : ViewModel, IRecordProvider<ReferencedPlacedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IReferenceService _referenceService;

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public IEnumerable<Type> RecordTypes { get; } = [typeof(IPlacedGetter)];
    [Reactive] public partial FormKey CellFormKey { get; set; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; set; }

    public PlacedProvider(
        ILinkCacheProvider linkCacheProvider,
        IRecordController recordController,
        IReferenceService referenceService,
        IRecordBrowserSettings recordBrowserSettings) {
        _linkCacheProvider = linkCacheProvider;
        _referenceService = referenceService;
        RecordBrowserSettings = recordBrowserSettings;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        this.WhenAnyValue(x => x.CellFormKey)
            .ObserveOnTaskpool()
            .WrapInProgressMarker(x => x.Do(_ => UpdatePlacedRecords()), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.WinningRecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(x => UpdatePlacedRecord(x.Record, x.Mod))
            .DisposeWith(this);

        recordController.WinningRecordDeleted
            .Subscribe(x => RemovePlacedRecord(x.Record, x.Mod))
            .DisposeWith(this);
    }

    private void UpdatePlacedRecords() {
        _referencesDisposable.Clear();

        RecordCache.Clear();

        if (CellFormKey == FormKey.Null) return;

        // Add all references in the cell from all overrides of the cell of all cells.
        // Starting with the most prioritized cell and keep track of which references have already been added to avoid duplicates.
        HashSet<FormKey> refFormKeys = [];
        foreach (var cell in _linkCacheProvider.LinkCache.ResolveAll<ICellGetter>(CellFormKey)) {
            RecordCache.Edit(updater => {
                foreach (var record in cell.GetAllPlaced(_linkCacheProvider.LinkCache)) {
                    if (!refFormKeys.Add(record.FormKey)) continue;

                    _referenceService.GetReferencedRecord(record, out var referencedRecord).DisposeWithComposite(_referencesDisposable);
                    var referencedPlacedRecord = new ReferencedPlacedRecord(referencedRecord, _linkCacheProvider.LinkCache);

                    updater.AddOrUpdate(referencedPlacedRecord);
                }
            });
        }
    }

    private void UpdatePlacedRecord(IMajorRecord record, IMod mod) {
        if (record is not IPlacedGetter placed) return;

        if (RecordCache.TryGetValue(placed.FormKey, out var referencedPlaced)) {
            // Modify value
            referencedPlaced.Record = placed;
        } else {
            // Create new entry
            _referenceService.GetReferencedRecord(placed, out var outReferencedPlaced).DisposeWithComposite(_referencesDisposable);
            referencedPlaced = outReferencedPlaced;
        }

        // Force update
        RecordCache.AddOrUpdate(referencedPlaced);
    }

    private void RemovePlacedRecord(IMajorRecord record, IMod mod) {
        if (record is not IPlacedGetter placed) return;

        RecordCache.RemoveKey(placed.FormKey);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
