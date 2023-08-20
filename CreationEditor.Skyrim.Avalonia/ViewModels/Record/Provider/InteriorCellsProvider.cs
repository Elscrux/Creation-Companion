using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Avalonia.Services.Record.Actions;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class InteriorCellsProvider : ViewModel, IRecordProvider<IReferencedRecord<ICellGetter>>, ICellLoadStrategy {
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

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }
    public IRecordContextMenuProvider RecordContextMenuProvider { get; }

    public InteriorCellsProvider(
        IViewportRuntimeService viewportRuntimeService,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordBrowserSettings recordBrowserSettings,
        Func<IObservable<ICellGetter?>, ICellLoadStrategy, CellContextMenuProvider> cellContextMenuProviderFactory) {
        _viewportRuntimeService = viewportRuntimeService;
        RecordBrowserSettings = recordBrowserSettings;
        var selectedCellObservable = this.WhenAnyValue(x => x.SelectedRecord)
            .Select(x => x?.Record);
        RecordContextMenuProvider = cellContextMenuProviderFactory(selectedCellObservable, this);

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(linkCache => {
                _referencesDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in linkCache.PriorityOrder.WinningOverrides<ICellGetter>()) {
                        if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) continue;

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
                } else {
                    // Check if cell exists in current scope
                    if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) return;

                    // Create new entry
                    recordReferenceController.GetReferencedRecord(cell, out var outListRecord).DisposeWith(_referencesDisposable);
                    listRecord = outListRecord;
                }

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        recordController.RecordDeleted
            .OfType<ICellGetter>()
            .Subscribe(record => RecordCache.RemoveKey(record.FormKey))
            .DisposeWith(this);
    }

    public void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadInteriorCell(cell);
    }

    public override void Dispose() {
        base.Dispose();

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
