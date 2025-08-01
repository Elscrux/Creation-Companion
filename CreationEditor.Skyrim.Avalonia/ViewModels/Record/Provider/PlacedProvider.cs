﻿using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed partial class PlacedProvider : ViewModel, IRecordProvider<ReferencedPlacedRecord> {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public IEnumerable<Type> RecordTypes { get; } = [typeof(IPlacedGetter)];
    [Reactive] public partial FormKey CellFormKey { get; set; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; set; }

    public PlacedProvider(
        ILinkCacheProvider linkCacheProvider,
        IRecordController recordController,
        IReferenceService referenceService,
        IRecordBrowserSettings recordBrowserSettings) {
        RecordBrowserSettings = recordBrowserSettings;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        this.WhenAnyValue(x => x.CellFormKey)
            .ObserveOnTaskpool()
            .WrapInProgressMarker(
                x => x.Do(_ => {
                    _referencesDisposable.Clear();

                    RecordCache.Clear();

                    if (CellFormKey == FormKey.Null) return;

                    // Add all references in the cell from all overrides of the cell of all cells.
                    // Starting with the most prioritized cell and keep track of which references have already been added to avoid duplicates.
                    HashSet<FormKey> refFormKeys = [];
                    foreach (var cell in linkCacheProvider.LinkCache.ResolveAll<ICellGetter>(CellFormKey)) {
                        RecordCache.Edit(updater => {
                            foreach (var record in cell.GetAllPlaced(linkCacheProvider.LinkCache)) {
                                if (!refFormKeys.Add(record.FormKey)) continue;

                                referenceService.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);
                                var referencedPlacedRecord = new ReferencedPlacedRecord(referencedRecord, linkCacheProvider.LinkCache);

                                updater.AddOrUpdate(referencedPlacedRecord);
                            }
                        });
                    }
                }),
                out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.WinningRecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(x => {
                if (x.Record is not IPlacedGetter placed) return;

                if (RecordCache.TryGetValue(placed.FormKey, out var referencedPlaced)) {
                    // Modify value
                    referencedPlaced.Record = placed;
                } else {
                    // Create new entry
                    referenceService.GetReferencedRecord(placed, out var outReferencedPlaced).DisposeWith(_referencesDisposable);
                    referencedPlaced = outReferencedPlaced;
                }

                // Force update
                RecordCache.AddOrUpdate(referencedPlaced);
            })
            .DisposeWith(this);

        recordController.WinningRecordDeleted
            .Subscribe(x => {
                if (x.Record is not IPlacedGetter placed) return;

                RecordCache.RemoveKey(placed.FormKey);
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
