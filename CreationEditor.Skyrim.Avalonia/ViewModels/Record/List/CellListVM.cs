using System;
using System.Reactive;
using System.Threading;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using DynamicData;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List; 

public abstract class CellListVM : ARecordListVM<ReferencedRecord<Cell, ICellGetter>> {
    public ReactiveCommand<Unit, Unit> ViewSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> NewCell { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedCell { get; }

    public CellListVM(
        MainWindow mainWindow,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery,
        IRecordController recordController,
        IDockFactory dockFactory,
        IRecordEditorController recordEditorController)
        : base(mainWindow, recordListFactory, recordBrowserSettingsVM, referenceQuery, recordController) {
        
        ViewSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            dockFactory.Open(DockElement.Viewport);

            Thread.Sleep(250);

            LoadCell(SelectedRecord.Record);
        });
        
        NewCell = ReactiveCommand.Create(() => {
            var newRecord = RecordController.CreateRecord<Cell, ICellGetter>();
            recordEditorController.OpenEditor<Cell, ICellGetter>(newRecord);
            
            var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(newRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });

        EditSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = RecordController.GetOrAddOverride<Cell, ICellGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<Cell, ICellGetter>(newOverride);
        });

        DuplicateSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var duplicate = RecordController.DuplicateRecord<Cell, ICellGetter>(SelectedRecord.Record);

            var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(duplicate);
            RecordCache.AddOrUpdate(referencedRecord);
        });
        
        DeleteSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            RecordController.DeleteRecord<Cell, ICellGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        recordEditorController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not ICellGetter record) return;
                if (!RecordCache.TryGetValue(record.FormKey, out var listRecord)) return;
                
                // Modify value
                listRecord.Record = record;
                
                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        ContextMenuItems.Insert(0,new MenuItem { Header = "View", Command = ViewSelectedCell });
        ContextMenuItems.Insert(1,new MenuItem { Header = "New", Command = NewCell });
        ContextMenuItems.Insert(2,new MenuItem { Header = "Edit", Command = EditSelectedCell });
        ContextMenuItems.Insert(3,new MenuItem { Header = "Duplicate", Command = DuplicateSelectedCell });
        ContextMenuItems.Insert(4,new MenuItem { Header = "Delete", Command = DeleteSelectedCell });

        DoubleTapCommand = ViewSelectedCell;
    }

    protected abstract void LoadCell(ICellGetter cell);
}