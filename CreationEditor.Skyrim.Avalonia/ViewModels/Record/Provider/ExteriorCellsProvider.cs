using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Picker;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class ExteriorCellsProvider : ViewModel, IRecordProvider<IReferencedRecord<ICellGetter>> {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    public IRecordPickerVM WorldSpacePickerVM { get; }
    [Reactive] public FormKey WorldspaceFormKey { get; set; }
    [Reactive] public bool ShowWildernessCells { get; set; } = true;

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public IObservable<bool> IsBusy { get; }

    public ExteriorCellsProvider(
        ILinkCacheProvider linkCacheProvider,
        IRecordReferenceController recordReferenceController,
        IRecordController recordController,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordPickerVM worldSpacePickerVM) {
        RecordBrowserSettings = recordBrowserSettings;
        WorldSpacePickerVM = worldSpacePickerVM;
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

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
