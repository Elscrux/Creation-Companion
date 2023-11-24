using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class PlacedProvider : ViewModel, IRecordProvider<ReferencedPlacedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    [Reactive] public FormKey CellFormKey { get; set; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; set; }

    public PlacedProvider(
        ILinkCacheProvider linkCacheProvider,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordBrowserSettings recordBrowserSettings) {
        RecordBrowserSettings = recordBrowserSettings;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        this.WhenAnyValue(x => x.CellFormKey)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(_ => {
                _referencesDisposable.Clear();

                RecordCache.Clear();

                if (CellFormKey == FormKey.Null) return;

                // Add all references in the cell from all overrides of the cell of all cells.
                // Starting with the most prioritized cell and keep track of which references have already been added to avoid duplicates.
                HashSet<FormKey> refFormKeys = [];
                foreach (var cell in linkCacheProvider.LinkCache.ResolveAll<ICellGetter>(CellFormKey)) {
                    RecordCache.Edit(updater => {
                        foreach (var record in cell.Temporary.Concat(cell.Persistent)) {
                            if (!refFormKeys.Add(record.FormKey)) continue;

                            recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);
                            var referencedPlacedRecord = new ReferencedPlacedRecord(referencedRecord, RecordBrowserSettings.ModScopeProvider.LinkCache);

                            updater.AddOrUpdate(referencedPlacedRecord);
                        }
                    });
                }
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.RecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(majorRecord => {
                if (majorRecord is not IPlacedGetter record) return;

                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    recordReferenceController.GetReferencedRecord(record, out var outListRecord).DisposeWith(this);
                    listRecord = outListRecord;
                }

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
