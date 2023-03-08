using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Controller;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class PlacedProvider : ViewModel, IRecordProvider<ReferencedPlacedRecord> {
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
        IRecordEditorController recordEditorController,
        IRecordController recordController,
        IReferenceController referenceController,
        IRecordBrowserSettingsVM recordBrowserSettingsVM) {
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = recordController.CreateRecord<IPlaced, IPlacedGetter>();
            recordEditorController.OpenEditor<IPlaced, IPlacedGetter>(newRecord);

            referenceController.GetRecord(newRecord, out var referencedRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });

        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = recordController.DuplicateRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<IPlaced, IPlacedGetter>(newOverride);
        });

        EditSelectedRecordBase = ReactiveCommand.Create(() => {
            if (SelectedRecord?.Record is not IPlacedObjectGetter placedObject) return;

            var placeable = placedObject.Base.TryResolve(RecordBrowserSettingsVM.LinkCache);
            if (placeable == null) return;

            var newOverride = recordController.GetOrAddOverride<IPlaceableObject, IPlaceableObjectGetter>(placeable);
            recordEditorController.OpenEditor(newOverride);
        });

        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var duplicate = recordController.DuplicateRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);

            referenceController.GetRecord(duplicate, out var referencedRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });

        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            recordController.DeleteRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        var cacheDisposable = new CompositeDisposable();

        this.WhenAnyValue(x => x.Cell)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(_ => {
                cacheDisposable.Clear();

                RecordCache.Clear();

                if (Cell == null) return;

                RecordCache.Edit(updater => {
                    foreach (var record in Cell.Temporary.Concat(Cell.Persistent)) {
                        var references = referenceController.ReferenceCache?.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                        var referencedRecord = new ReferencedPlacedRecord(record, RecordBrowserSettingsVM.LinkCache, references);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not IPlacedGetter record) return;

                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    referenceController.GetRecord(record, out var outListRecord);
                    listRecord = outListRecord;
                }

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        ContextMenuItems = new List<IMenuItem> {
            new MenuItem { Header = "New", Command = NewRecord },
            new MenuItem { Header = "Edit", Command = EditSelectedRecord },
            new MenuItem { Header = "Edit Base", Command = EditSelectedRecordBase },
            new MenuItem { Header = "Duplicate", Command = DuplicateSelectedRecord },
            new MenuItem { Header = "Delete", Command = DeleteSelectedRecord },
        };
    }
}
