using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider; 

public sealed class PlacedProvider : ViewModel, IRecordProvider<ReferencedRecord<IPlaced, IPlacedGetter>> {
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
    [Reactive] public ICellGetter? Cell { get; set; }
    
    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);
    
    [Reactive] public ReferencedRecord<IPlaced, IPlacedGetter>? SelectedRecord { get; set; }
    IReferencedRecord? IRecordProvider.SelectedRecord {
        get => SelectedRecord;
        set {
            if (value is ReferencedRecord<IPlaced, IPlacedGetter> referencedRecord) {
                SelectedRecord = referencedRecord;
            }
        }
    }
    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    [Reactive] public bool IsBusy { get; set; }
    
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
        IReferenceQuery referenceQuery,
        IRecordBrowserSettingsVM recordBrowserSettingsVM) {
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = recordController.CreateRecord<IPlaced, IPlacedGetter>();
            recordEditorController.OpenEditor<IPlaced, IPlacedGetter>(newRecord);

            var referencedRecord = new ReferencedRecord<IPlaced, IPlacedGetter>(newRecord);
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

            var referencedRecord = new ReferencedRecord<IPlaced, IPlacedGetter>(duplicate);
            RecordCache.AddOrUpdate(referencedRecord);
        });

        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            recordController.DeleteRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });
        
        this.WhenAnyValue(x => x.Cell)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(_ => {
                RecordCache.Clear();
                RecordCache.Refresh();

                try {
                    if (Cell == null) return;

                    RecordCache.Edit(updater => {
                        foreach (var record in Cell.Temporary.Concat(Cell.Persistent)) {
                            var referencedRecord = new ReferencedPlacedRecord(record, RecordBrowserSettingsVM.LinkCache, referenceQuery);

                            updater.AddOrUpdate(referencedRecord);
                        }
                    });
                } finally {
                    Dispatcher.UIThread.Post(() => IsBusy = false);
                }
            })
            .DisposeWith(this);

        recordController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not IPlacedGetter record) return;
                
                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    listRecord = new ReferencedRecord<IPlaced, IPlacedGetter>(record, RecordBrowserSettingsVM.LinkCache, referenceQuery);
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
