using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
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

    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
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

    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand => null;

    public ReactiveCommand<Unit, Unit> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecordBase { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }

    public PlacedProvider(
        IMenuItemProvider menuItemProvider,
        IRecordEditorController recordEditorController,
        IRecordController recordController,
        IRecordReferenceController recordReferenceController,
        IRecordBrowserSettingsVM recordBrowserSettingsVM) {
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = recordController.CreateRecord<IPlaced, IPlacedGetter>();
            recordEditorController.OpenEditor<IPlaced, IPlacedGetter>(newRecord);
        });

        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            var newOverride = recordController.DuplicateRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<IPlaced, IPlacedGetter>(newOverride);
        });

        EditSelectedRecordBase = ReactiveCommand.Create(() => {
            if (SelectedRecord?.Record is not IPlacedObjectGetter placedObject) return;

            var placeable = placedObject.Base.TryResolve(RecordBrowserSettingsVM.LinkCache);
            if (placeable is null) return;

            var newOverride = recordController.GetOrAddOverride<IPlaceableObject, IPlaceableObjectGetter>(placeable);
            recordEditorController.OpenEditor(newOverride);
        });

        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            recordController.DuplicateRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
        });

        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            recordController.DeleteRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        this.WhenAnyValue(x => x.Cell)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(_ => {
                _referencesDisposable.Clear();

                RecordCache.Clear();

                if (Cell is null) return;

                RecordCache.Edit(updater => {
                    foreach (var record in Cell.Temporary.Concat(Cell.Persistent)) {
                        recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);
                        var referencedPlacedRecord = new ReferencedPlacedRecord(referencedRecord, RecordBrowserSettingsVM.LinkCache);

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

        ContextMenuItems = new List<IMenuItem> {
            menuItemProvider.New(NewRecord),
            menuItemProvider.Edit(EditSelectedRecord),
            new MenuItem { Header = "Edit Base", Command = EditSelectedRecordBase },
            menuItemProvider.Duplicate(DuplicateSelectedRecord),
            menuItemProvider.Delete(DeleteSelectedRecord),
        };
    }

    public override void Dispose() {
        base.Dispose();

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
