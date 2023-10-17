using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Avalonia.Services.Record.Actions;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class ExteriorCellsProvider : ViewModel, IRecordProvider<IReferencedRecord<ICellGetter>>, ICellLoadStrategy {
    private readonly CompositeDisposable _referencesDisposable = new();
    private readonly IViewportRuntimeService _viewportRuntimeService;

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    [Reactive] public IReferencedRecord<ICellGetter>? SelectedRecord { get; set; }
    IReferencedRecord? IRecordProvider.SelectedRecord {
        get => SelectedRecord;
        set {
            if (value is IReferencedRecord<ICellGetter> referencedRecord) {
                SelectedRecord = referencedRecord;
            }
        }
    }

    [Reactive] public FormKey WorldspaceFormKey { get; set; }
    [Reactive] public bool ShowWildernessCells { get; set; } = true;

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }
    public IRecordContextMenuProvider RecordContextMenuProvider { get; }

    public ExteriorCellsProvider(
        ILinkCacheProvider linkCacheProvider,
        IViewportRuntimeService viewportRuntimeService,
        IRecordReferenceController recordReferenceController,
        IRecordController recordController,
        IRecordBrowserSettings recordBrowserSettings,
        Func<IObservable<ICellGetter?>, ICellLoadStrategy, CellContextMenuProvider> cellContextMenuProviderFactory) {
        _viewportRuntimeService = viewportRuntimeService;
        RecordBrowserSettings = recordBrowserSettings;
        var selectedCellObservable = this.WhenAnyValue(x => x.SelectedRecord)
            .Select(x => x?.Record);
        RecordContextMenuProvider = cellContextMenuProviderFactory(selectedCellObservable, this);

        Filter = RecordBrowserSettings.SettingsChanged
            .Merge(this.WhenAnyValue(x => x.ShowWildernessCells).Unit())
            .Select(_ => new Func<IReferencedRecord, bool>(
                record => (ShowWildernessCells || !record.Record.EditorID.IsNullOrEmpty()) && RecordBrowserSettings.Filter(record.Record)));

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged
            .CombineLatest(
                this.WhenAnyValue(x => x.WorldspaceFormKey),
                (linkCache, worldspaceFormKey) => (LinkCache: linkCache, WorldspaceFormKey: worldspaceFormKey))
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(y => {
                _referencesDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in y.LinkCache.EnumerateAllCells(y.WorldspaceFormKey)) {
                        recordReferenceController.GetReferencedRecord(cell, out var referencedRecord).DisposeWith(_referencesDisposable);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.RecordChanged
            .Merge(recordController.RecordCreated)
            .OfType<ICellGetter>()
            .Subscribe(cell => {
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
                    recordReferenceController.GetReferencedRecord(cell, out var outListRecord).DisposeWith(this);
                    listRecord = outListRecord;

                    // Force update
                    RecordCache.AddOrUpdate(listRecord);
                }
            })
            .DisposeWith(this);

        recordController.RecordDeleted
            .OfType<ICellGetter>()
            .Subscribe(record => RecordCache.RemoveKey(record.FormKey))
            .DisposeWith(this);
    }

    public void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadExteriorCell(WorldspaceFormKey, cell);
    }

    public void TrySelect(ICellGetter cell) {
        if (!RecordCache.TryGetValue(cell.FormKey, out var referencedRecord)) return;
        if (referencedRecord is not IReferencedRecord<ICellGetter> referencedCell) return;

        SelectedRecord = null;
        SelectedRecord = referencedCell;
    }

    public override void Dispose() {
        base.Dispose();

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
