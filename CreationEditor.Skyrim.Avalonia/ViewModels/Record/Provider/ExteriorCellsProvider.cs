using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.References.Controller;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using CreationEditor.Skyrim.Extension;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class ExteriorCellsProvider : CellProvider {
    private readonly IViewportRuntimeService _viewportRuntimeService;

    [Reactive] public FormKey WorldspaceFormKey { get; set; }
    [Reactive] public bool ShowWildernessCells { get; set; } = true;

    public override IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public override IObservable<bool> IsBusy { get; set; }

    public ExteriorCellsProvider(
        IRecordController recordController,
        IDockFactory dockFactory,
        IRecordEditorController recordEditorController,
        IViewportRuntimeService viewportRuntimeService,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceController referenceController)
        : base(recordController, dockFactory, recordEditorController, recordBrowserSettingsVM, referenceController) {
        _viewportRuntimeService = viewportRuntimeService;

        Filter = RecordBrowserSettingsVM.SettingsChanged
            .Merge(this.WhenAnyValue(x => x.ShowWildernessCells).Unit())
            .Select(_ => new Func<IReferencedRecord, bool>(
                record => (ShowWildernessCells || !record.Record.EditorID.IsNullOrEmpty()) && RecordBrowserSettingsVM.Filter(record.Record)));

        var cacheDisposable = new CompositeDisposable();

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache, x => x.WorldspaceFormKey)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(_ => {
                cacheDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in RecordBrowserSettingsVM.LinkCache.EnumerateAllCells(WorldspaceFormKey)) {
                        cacheDisposable.Add(referenceController.GetRecord(cell, out var referencedRecord));

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;
    }

    protected override void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadExteriorCell(WorldspaceFormKey, cell);
    }
}
