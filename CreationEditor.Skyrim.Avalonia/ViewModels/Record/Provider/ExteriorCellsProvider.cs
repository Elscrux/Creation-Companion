using System;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using CreationEditor.Skyrim.Extension;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public class ExteriorCellsProvider : CellProvider {
    private readonly IViewportRuntimeService _viewportRuntimeService;

    [Reactive] public FormKey WorldspaceFormKey { get; set; }
    [Reactive] public bool ShowWildernessCells { get; set; } = true;
    
    public override IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public ExteriorCellsProvider(
        IRecordController recordController,
        IDockFactory dockFactory,
        IRecordEditorController recordEditorController,
        IViewportRuntimeService viewportRuntimeService,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery)
        : base(recordController, dockFactory, recordEditorController, recordBrowserSettingsVM, referenceQuery) {
        _viewportRuntimeService = viewportRuntimeService;

        Filter = RecordBrowserSettingsVM.SettingsChanged
            .Select(_ => new Func<IReferencedRecord, bool>(
                record => (ShowWildernessCells || !record.Record.EditorID.IsNullOrEmpty()) && RecordBrowserSettingsVM.Filter(record.Record)));

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache, x => x.WorldspaceFormKey)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(_ => {
                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in RecordBrowserSettingsVM.LinkCache.EnumerateAllCells(WorldspaceFormKey)) {
                        var formLinks = referenceQuery.GetReferences(cell.FormKey, RecordBrowserSettingsVM.LinkCache);
                        var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(cell, formLinks);
                        updater.AddOrUpdate(referencedRecord);
                    }
                });

                Dispatcher.UIThread.Post(() => IsBusy = false);
            })
            .DisposeWith(this);
    }

    protected override void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadExteriorCell(WorldspaceFormKey, cell);
    }
}