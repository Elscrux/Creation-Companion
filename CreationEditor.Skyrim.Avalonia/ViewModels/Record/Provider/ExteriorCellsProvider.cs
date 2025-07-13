using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed partial class ExteriorCellsProvider : ViewModel, IRecordProvider<IReferencedRecord<ICellGetter>> {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public IEnumerable<Type> RecordTypes { get; } = [typeof(ICellGetter)];
    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    [Reactive] public partial FormKey WorldspaceFormKey { get; set; }
    [Reactive] public partial bool ShowWildernessCells { get; set; }

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }

    public ExteriorCellsProvider(
        ILinkCacheProvider linkCacheProvider,
        IReferenceService referenceService,
        IRecordController recordController,
        IRecordBrowserSettings recordBrowserSettings) {
        RecordBrowserSettings = recordBrowserSettings;
        ShowWildernessCells = true;
        Filter = RecordBrowserSettings.SettingsChanged
            .Merge(this.WhenAnyValue(x => x.ShowWildernessCells).Unit())
            .Select(_ => new Func<IReferencedRecord, bool>(
                record => (ShowWildernessCells || !record.Record.EditorID.IsNullOrEmpty()) && RecordBrowserSettings.Filter(record.Record)));

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged
            .CombineLatest(
                this.WhenAnyValue(x => x.WorldspaceFormKey),
                (linkCache, worldspaceFormKey) => (LinkCache: linkCache, WorldspaceFormKey: worldspaceFormKey))
            .ThrottleMedium()
            .ObserveOnTaskpool()
            .WrapInProgressMarker(
                x => x.Do(y => {
                    _referencesDisposable.Clear();

                    RecordCache.Clear();
                    RecordCache.Edit(updater => {
                        foreach (var cell in y.LinkCache.EnumerateAllCells(y.WorldspaceFormKey)) {
                            referenceService.GetReferencedRecord(cell, out var referencedRecord).DisposeWith(_referencesDisposable);

                            updater.AddOrUpdate(referencedRecord);
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
                if (x.Record is not ICellGetter cell) return;

                if (RecordCache.TryGetValue(cell.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = cell;

                    // Force update
                    RecordCache.AddOrUpdate(listRecord);
                } else {
                    // Check if cell exists in current scope
                    if ((cell.Flags & Cell.Flag.IsInteriorCell) != 0) return;
                    if (!linkCacheProvider.LinkCache.TryResolveSimpleContext<ICellGetter>(cell.FormKey, out var cellContext)) return;
                    if (cellContext.Parent?.Record is not IWorldspaceGetter worldspace) return;
                    if (worldspace.FormKey != WorldspaceFormKey) return;

                    // Create new entry
                    referenceService.GetReferencedRecord(cell, out var outListRecord).DisposeWith(_referencesDisposable);
                    listRecord = outListRecord;

                    // Force update
                    RecordCache.AddOrUpdate(listRecord);
                }
            })
            .DisposeWith(this);

        recordController.WinningRecordDeleted
            .Subscribe(x => {
                if (x.Record is not ICellGetter cell) return;

                RecordCache.RemoveKey(cell.FormKey);
            })
            .DisposeWith(this);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
