using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public sealed class RecordIdentifiersProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();

    [Reactive] public IEnumerable<IFormLinkIdentifier> Identifiers { get; set; }
    public IEnumerable<Type> RecordTypes => _recordTypes.ToList();
    private readonly HashSet<Type> _recordTypes = [];
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }

    public RecordIdentifiersProvider(
        IEnumerable<IFormLinkIdentifier> identifiers,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        ILogger logger) {
        Identifiers = identifiers;
        RecordBrowserSettings = recordBrowserSettings;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged.CombineLatest(
                this.WhenAnyValue(x => x.Identifiers),
                (linkCache, idents) => (LinkCache: linkCache, Identifiers: idents))
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(w => w.Do(x => {
                _referencesDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var identifier in x.Identifiers) {
                        var formKey = identifier.FormKey;
                        _recordTypes.Add(identifier.Type);
                        if (x.LinkCache.TryResolve(formKey, identifier.Type, out var record)) {
                            recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);

                            updater.AddOrUpdate(referencedRecord);
                        } else {
                            logger.Here().Error("Couldn't load form link {FormKey} - {Type}", formKey, identifier.Type);
                        }
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.RecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(record => {
                // Don't add if record not in the original identifiers list
                if (!RecordCache.TryGetValue(record.FormKey, out var listRecord)) return;

                // Modify value
                listRecord.Record = record;

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        recordController.RecordDeleted
            .Subscribe(record => RecordCache.RemoveKey(record.FormKey))
            .DisposeWith(this);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
