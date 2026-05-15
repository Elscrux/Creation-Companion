using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public sealed partial class RecordIdentifiersProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();
    private readonly IReferenceService _referenceService;
    private readonly ILogger _logger;

    [Reactive] public partial IEnumerable<IFormLinkIdentifier> Identifiers { get; set; }
    public IEnumerable<Type> RecordTypes => _recordTypes;
    private readonly HashSet<Type> _recordTypes = [];
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }

    public RecordIdentifiersProvider(
        IEnumerable<IFormLinkIdentifier> identifiers,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordController recordController,
        IReferenceService referenceService,
        ILogger logger) {
        Identifiers = identifiers;
        RecordBrowserSettings = recordBrowserSettings;
        _referenceService = referenceService;
        _logger = logger;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged.CombineLatest(
                this.WhenAnyValue(x => x.Identifiers),
                (linkCache, idents) => (LinkCache: linkCache, Identifiers: idents))
            .ObserveOnTaskpool()
            .WrapInProgressMarker(w => w.Do(x => UpdateRecordCache(x.LinkCache, x.Identifiers)),
                out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.WinningRecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(x => UpdateRecord(x.Record, x.Mod))
            .DisposeWith(this);

        recordController.WinningRecordDeleted
            .Subscribe(x => RemoveRecord(x.Record, x.Mod))
            .DisposeWith(this);
    }

    private void UpdateRecordCache(ILinkCache linkCache, IEnumerable<IFormLinkIdentifier> identifiers) {
        _referencesDisposable.Clear();

        RecordCache.Clear();
        RecordCache.Edit(updater => {
            foreach (var identifier in identifiers) {
                var formKey = identifier.FormKey;
                _recordTypes.Add(identifier.Type);
                if (linkCache.TryResolve(formKey, identifier.Type, out var record)) {
                    _referenceService.GetReferencedRecord(record, out var referencedRecord).DisposeWithComposite(_referencesDisposable);

                    updater.AddOrUpdate(referencedRecord);
                } else {
                    _logger.Here().Error("Couldn't load form link {FormKey} - {Type}", formKey, identifier.Type);
                }
            }
        });
    }

    private void UpdateRecord(IMajorRecordGetter record, IModGetter _mod) {
        // Don't add if record not in the original identifiers list
        if (!RecordCache.TryGetValue(record.FormKey, out var listRecord)) return;

        // Modify value
        listRecord.Record = record;

        // Force update
        RecordCache.AddOrUpdate(listRecord);
    }

    private void RemoveRecord(IMajorRecordGetter record, IModGetter _mod) {
        RecordCache.RemoveKey(record.FormKey);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
