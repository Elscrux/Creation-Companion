using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Autofac;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
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
        ILifetimeScope lifetimeScope,
        IViewportRuntimeService viewportRuntimeService,
        IRecordReferenceController recordReferenceController)
        : base(lifetimeScope) {
        _viewportRuntimeService = viewportRuntimeService;

        Filter = RecordBrowserSettings.SettingsChanged
            .Merge(this.WhenAnyValue(x => x.ShowWildernessCells).Unit())
            .Select(_ => new Func<IReferencedRecord, bool>(
                record => (ShowWildernessCells || !record.Record.EditorID.IsNullOrEmpty()) && RecordBrowserSettings.Filter(record.Record)));

        RecordBrowserSettings.ModScopeProvider.LinkCacheChanged
            .CombineLatest(
                this.WhenAnyValue(x => x.WorldspaceFormKey),
                (linkCache, worldspaceFormKey) => (LinkCache: linkCache, WorldspaceFormKey: worldspaceFormKey))
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(y => {
                ReferencesDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var cell in y.LinkCache.EnumerateAllCells(y.WorldspaceFormKey)) {
                        recordReferenceController.GetReferencedRecord(cell, out var referencedRecord).DisposeWith(ReferencesDisposable);

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
