using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;

public sealed class InteriorCellsProvider : CellProvider {
    private readonly IViewportRuntimeService _viewportRuntimeService;

    public override IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    public override IObservable<bool> IsBusy { get; set; }

    public InteriorCellsProvider(
        ILifetimeScope lifetimeScope,
        IViewportRuntimeService viewportRuntimeService,
        IRecordReferenceController recordReferenceController)
        : base(lifetimeScope) {
        _viewportRuntimeService = viewportRuntimeService;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(linkCache => {
                ReferencesDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in linkCache.PriorityOrder.WinningOverrides<ICellGetter>()) {
                        if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) continue;

                        recordReferenceController.GetRecord(cell, out var referencedRecord).DisposeWith(ReferencesDisposable);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;
    }

    protected override void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadInteriorCell(cell);
    }
}
