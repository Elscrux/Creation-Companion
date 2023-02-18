using System;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public class InteriorCellsProvider : CellProvider {
    private readonly IViewportRuntimeService _viewportRuntimeService;
    
    public override IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public InteriorCellsProvider(
        IRecordController recordController,
        IDockFactory dockFactory,
        IRecordEditorController recordEditorController,
        IViewportRuntimeService viewportRuntimeService,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery)
        : base(recordController, dockFactory, recordEditorController, recordBrowserSettingsVM, referenceQuery) {
        _viewportRuntimeService = viewportRuntimeService;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(linkCache => {
                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in linkCache.PriorityOrder.WinningOverrides<ICellGetter>()) {
                        if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) continue;
                        
                        var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(cell, RecordBrowserSettingsVM.LinkCache, referenceQuery);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });

                Dispatcher.UIThread.Post(() => IsBusy = false);
            })
            .DisposeWith(this);
    }

    protected override void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadInteriorCell(cell);
    }
}
