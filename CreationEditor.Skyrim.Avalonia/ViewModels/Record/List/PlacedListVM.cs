using System;
using System.Linq;
using System.Reactive;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Models.Record;
using DynamicData;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List; 

public interface IPlacedListVM {
    public RecordList? PlacedList { get; }

    public ICellGetter? Cell { get; set; }
}

public sealed class PlacedListVM : ARecordListVM<ReferencedRecord<IPlaced, IPlacedGetter>>, IPlacedListVM {
    [Reactive] public ICellGetter? Cell { get; set; }
    [Reactive] public bool Filled { get; set; }

    public ReactiveCommand<Unit, Unit> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }
    
    public RecordList? PlacedList { get; }

    public PlacedListVM(
        MainWindow mainWindow,
        IExtraColumnProvider extraColumnProvider,
        IReferenceQuery referenceQuery,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IRecordEditorController recordEditorController,
        IRecordController recordController)
        : base(mainWindow, recordListFactory, recordBrowserSettingsVM, referenceQuery, recordController) {
        
        PlacedList = new RecordList(extraColumnProvider.GetColumns(typeof(IPlacedGetter))) { DataContext = this };

        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = RecordController.CreateRecord<IPlaced, IPlacedGetter>();
            recordEditorController.OpenEditor<IPlaced, IPlacedGetter>(newRecord);

            var referencedRecord = new ReferencedRecord<IPlaced, IPlacedGetter>(newRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });

        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = recordController.DuplicateRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<IPlaced, IPlacedGetter>(newOverride);
        });

        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var duplicate = RecordController.DuplicateRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);

            var referencedRecord = new ReferencedRecord<IPlaced, IPlacedGetter>(duplicate);
            RecordCache.AddOrUpdate(referencedRecord);
        });

        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            RecordController.DeleteRecord<IPlaced, IPlacedGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });
        
        this.WhenAnyValue(x => x.Cell)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(_ => {
                Filled = Cell != null;
                RecordCache.Clear();
                RecordCache.Refresh();

                try {
                    if (Cell == null) return;

                    RecordCache.Edit(updater => {
                        foreach (var record in Cell.Temporary.Concat(Cell.Persistent)) {
                            var formLinks = ReferenceQuery.GetReferences(record.FormKey, RecordBrowserSettingsVM.LinkCache);
                            var referencedRecord = new ReferencedPlacedRecord(record, RecordBrowserSettingsVM.LinkCache, formLinks);

                            updater.AddOrUpdate(referencedRecord);
                        }
                    });
                } finally {
                    Dispatcher.UIThread.Post(() => IsBusy = false);
                }
            })
            .DisposeWith(this);

        recordEditorController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not IPlacedGetter record) return;
                if (!RecordCache.TryGetValue(record.FormKey, out var listRecord)) return;
                
                // Modify value
                listRecord.Record = record;
                
                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);
    }
}
