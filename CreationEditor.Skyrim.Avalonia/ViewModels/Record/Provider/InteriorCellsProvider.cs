using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class InteriorCellsProvider : ViewModel, IRecordProvider<IReferencedRecord<ICellGetter>> {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public IEnumerable<Type> RecordTypes { get; } = [typeof(ICellGetter)];
    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }

    public InteriorCellsProvider(
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordBrowserSettings recordBrowserSettings) {
        RecordBrowserSettings = recordBrowserSettings;

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

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
