using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public sealed class RecordIdentifiersProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();

    [Reactive] public IEnumerable<IFormLinkIdentifier> Identifiers { get; set; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);
    [Reactive] public IReferencedRecord? SelectedRecord { get; set; }

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }
    public IRecordContextMenuProvider RecordContextMenuProvider { get; }

    public RecordIdentifiersProvider(
        IEnumerable<IFormLinkIdentifier> identifiers,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        ILogger logger,
        Func<IObservable<IMajorRecordGetter?>, UntypedRecordContextMenuProvider> untypedRecordContextMenuProviderFactory) {
        Identifiers = identifiers;
        RecordBrowserSettings = recordBrowserSettings;
        var selectedRecordObservable = this.WhenAnyValue(x => x.SelectedRecord)
            .Select(x => x?.Record);
        RecordContextMenuProvider = untypedRecordContextMenuProviderFactory(selectedRecordObservable);

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
                        if (x.LinkCache.TryResolve(formKey, identifier.Type, out var record)) {
                            recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);

                            updater.AddOrUpdate(referencedRecord);
                        } else {
                            logger.Error("Couldn't load form link {FormKey} - {Type}", formKey, identifier.Type);
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

    public override void Dispose() {
        base.Dispose();

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
