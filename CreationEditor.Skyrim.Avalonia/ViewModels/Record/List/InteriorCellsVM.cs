using System;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public class InteriorCellsVM : CellListVM {
    public RecordList InteriorList { get; }
    
    public InteriorCellsVM(
        IExtraColumnProvider extraColumnProvider,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery,
        IRecordController recordController,
        IDockFactory dockFactory,
        IViewportRuntimeService viewportRuntimeService,
        IRecordEditorController recordEditorController)
        : base(recordBrowserSettingsVM, referenceQuery, recordController, dockFactory, viewportRuntimeService, recordEditorController) {
        
        InteriorList = new RecordList(extraColumnProvider.GetColumns(typeof(ICellGetter))) { DataContext = this };

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(linkCache => {
                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in linkCache.PriorityOrder.WinningOverrides<ICellGetter>()) {
                        if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) continue;
                        
                        var formLinks = ReferenceQuery.GetReferences(cell.FormKey, RecordBrowserSettingsVM.LinkCache);
                        var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(cell, formLinks);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });

                Dispatcher.UIThread.Post(() => IsBusy = false);
            })
            .DisposeWith(this);
    }
}
