using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public sealed class RecordTypeProvider : ViewModel, IRecordProvider<IReferencedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IList<Type> Types { get; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public IEnumerable<Type> RecordTypes => Types;

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }

    public RecordTypeProvider(
        IEnumerable<Type> types,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController) {
        Types = types.ToList();
        RecordBrowserSettings = recordBrowserSettings;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged
            .ObserveOnTaskpool()
            .WrapInProgressMarker(x => x.Do(linkCache => {
                    _referencesDisposable.Clear();

                    RecordCache.Clear();
                    RecordCache.Edit(updater => {
                        foreach (var type in Types) {
                            foreach (var record in linkCache.PriorityOrder.WinningOverrides(type)) {
                                recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);

                                updater.AddOrUpdate(referencedRecord);
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
                if (!Types.Contains(x.Record.GetType())) return;

                if (RecordCache.TryGetValue(x.Record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = x.Record;
                } else {
                    // Create new entry
                    recordReferenceController.GetReferencedRecord(x.Record, out var outListRecord).DisposeWith(_referencesDisposable);
                    listRecord = outListRecord;
                }

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
