using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using CreationEditor.Skyrim.Avalonia.Services.Record.Actions;
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
    [Reactive] public ICellGetter? Cell { get; set; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    [Reactive] public ReferencedPlacedRecord? SelectedRecord { get; set; }
    IReferencedRecord? IRecordProvider.SelectedRecord {
        get => SelectedRecord;
        set {
            if (value is ReferencedPlacedRecord referencedRecord) {
                SelectedRecord = referencedRecord;
            }
        }
    }

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; set; }
    public IRecordContextMenuProvider RecordContextMenuProvider { get; }

    public PlacedProvider(
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordBrowserSettings recordBrowserSettings,
        Func<IObservable<IPlacedGetter?>, PlacedContextMenuProvider> placedContextMenuProviderFactory) {
        RecordBrowserSettings = recordBrowserSettings;
        var selectedPlacedObservable = this.WhenAnyValue(x => x.SelectedRecord)
            .Select(x => x?.Record);
        RecordContextMenuProvider = placedContextMenuProviderFactory(selectedPlacedObservable);

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettings);

        this.WhenAnyValue(x => x.Cell)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(_ => {
                _referencesDisposable.Clear();

                RecordCache.Clear();

                if (Cell is null) return;

                RecordCache.Edit(updater => {
                    foreach (var record in Cell.Temporary.Concat(Cell.Persistent)) {
                        recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);
                        var referencedPlacedRecord = new ReferencedPlacedRecord(referencedRecord, RecordBrowserSettings.ModScopeProvider.LinkCache);

                        updater.AddOrUpdate(referencedPlacedRecord);
                    }
                });
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
