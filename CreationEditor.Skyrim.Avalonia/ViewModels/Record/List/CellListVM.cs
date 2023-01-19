using System;
using System.Linq;
using System.Reactive;
using System.Threading;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using DynamicData;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List; 

public class CellListVM : ARecordListVM<ReferencedRecord<Cell, ICellGetter>> {
    public override Type Type => typeof(ICellGetter);
    
    public ReactiveCommand<Unit, Unit> ViewSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> NewCell { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedCell { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedCell { get; }

    [Reactive] public ReferencedRecord<Cell, ICellGetter>? SelectedRecord { get; set; }

    public CellListVM(
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery,
        IRecordController recordController,
        IDockFactory dockFactory,
        IViewportRuntimeService viewportRuntimeService,
        IRecordEditorController recordEditorController)
        : base(recordBrowserSettingsVM, referenceQuery, recordController) {
        
        ViewSelectedCell = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            dockFactory.Open(DockElement.Viewport);

            Thread.Sleep(250);
            
            viewportRuntimeService.Load(SelectedRecord.Record.Temporary.Concat(SelectedRecord.Record.Persistent), SelectedRecord.Record.Grid?.Point ?? new P2Int(0, 0));
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

        ContextMenuItems.Add(new MenuItem { Header = "View", Command = ViewSelectedCell });
        ContextMenuItems.Add(new MenuItem { Header = "New", Command = NewCell });
        ContextMenuItems.Add(new MenuItem { Header = "Edit", Command = EditSelectedCell });
        ContextMenuItems.Add(new MenuItem { Header = "Duplicate", Command = DuplicateSelectedCell });
        ContextMenuItems.Add(new MenuItem { Header = "Delete", Command = DeleteSelectedCell });

        DoubleTapCommand = ViewSelectedCell;
    }
}