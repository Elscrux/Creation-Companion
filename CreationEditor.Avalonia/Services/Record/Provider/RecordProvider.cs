using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public sealed class RecordProvider<TMajorRecord, TMajorRecordGetter> : ViewModel, IRecordProvider<IReferencedRecord<TMajorRecordGetter>>
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    private readonly CompositeDisposable _referencesDisposable = new();
    private readonly IReferenceService _referenceService;

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public IEnumerable<Type> RecordTypes { get; } = [typeof(TMajorRecordGetter)];

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }

    public RecordProvider(
        IRecordController recordController,
        IReferenceService referenceService,
        IRecordBrowserSettings recordBrowserSettings) {
        _referenceService = referenceService;
        RecordBrowserSettings = recordBrowserSettings;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged
            .ObserveOnTaskpool()
            .WrapInProgressMarker(x => x.Do(UpdateRecordCache),
                out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.WinningRecordChanged
            .Merge(recordController.RecordCreated)
            .ObserveOnGui()
            .Subscribe(x => UpdateRecord(x.Record, x.Mod))
            .DisposeWith(this);

        recordController.WinningRecordDeleted
            .Subscribe(x => RemoveRecord(x.Record, x.Mod))
            .DisposeWith(this);
    }

    private void UpdateRecordCache(ILinkCache linkCache) {
        _referencesDisposable.Clear();

        RecordCache.Clear();
        RecordCache.Edit(updater => {
            foreach (var record in linkCache.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                _referenceService.GetReferencedRecord(record, out var referencedRecord).DisposeWithComposite(_referencesDisposable);

                updater.AddOrUpdate(referencedRecord);
            }
        });
    }

    private void UpdateRecord(IMajorRecord record, IMod mod) {
        if (record is not TMajorRecordGetter r) return;

        if (RecordCache.TryGetValue(r.FormKey, out var listRecord)) {
            // Modify value
            listRecord.Record = r;
        } else {
            // Create new entry
            _referenceService.GetReferencedRecord(r, out var outListRecord).DisposeWithComposite(_referencesDisposable);
            listRecord = outListRecord;
        }

        // Force update
        RecordCache.AddOrUpdate(listRecord);
    }

    private void RemoveRecord(IMajorRecord record, IMod mod) {
        RecordCache.RemoveKey(record.FormKey);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
