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
using ReactiveUI.SourceGenerators;
using Serilog;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public sealed partial class RecordIdentifiersProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();

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
        IRecordReferenceController recordReferenceController,
        ILogger logger) {
        Identifiers = identifiers;
        RecordBrowserSettings = recordBrowserSettings;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged.CombineLatest(
                this.WhenAnyValue(x => x.Identifiers),
                (linkCache, idents) => (LinkCache: linkCache, Identifiers: idents))
            .ObserveOnTaskpool()
            .WrapInProgressMarker(w => w.Do(x => {
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
                }),
                out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.WinningRecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(x => {
                // Don't add if record not in the original identifiers list
                if (!RecordCache.TryGetValue(x.Record.FormKey, out var listRecord)) return;

                // Modify value
                listRecord.Record = x.Record;

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        recordController.WinningRecordDeleted
            .Subscribe(x => RecordCache.RemoveKey(x.Record.FormKey))
            .DisposeWith(this);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
