using System;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Skyrim.Avalonia.Services.Record.Actions;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class ExteriorCellsVM : ViewModel, ICellLoadStrategy {
    private readonly IViewportRuntimeService _viewportRuntimeService;
    public ExteriorCellsProvider ExteriorCellsProvider { get; }

    [Reactive] public int GridXValue { get; set; }
    [Reactive] public int GridYValue { get; set; }

    public IRecordListVM RecordListVM { get; }

    public ReactiveCommand<Unit, Unit> SelectGridCell { get; }

    public ExteriorCellsVM(
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        ExteriorCellsProvider exteriorCellsProvider,
        IViewportRuntimeService viewportRuntimeService,
        ILinkCacheProvider linkCacheProvider,
        Func<IObservable<ICellGetter?>, ICellLoadStrategy, CellContextMenuProvider> cellContextMenuProviderFactory) {
        _viewportRuntimeService = viewportRuntimeService;
        ExteriorCellsProvider = exteriorCellsProvider.DisposeWith(this);

        RecordListVM = recordListVMBuilder
            .WithExtraColumns(extraColumnsBuilder
                .AddRecordType<ICellGetter>()
                .AddColumnType<CellGridExtraColumns>())
            .WithContextMenuProviderFactory(CreateContextMenuProvider)
            .BuildWithSource(ExteriorCellsProvider)
            .DisposeWith(this);

        SelectGridCell = ReactiveCommand.Create(() => {
            if (!ExteriorCellsProvider.RecordBrowserSettings.ModScopeProvider.LinkCache.TryResolve<IWorldspaceGetter>(ExteriorCellsProvider.WorldspaceFormKey, out var worldspace)) return;

            foreach (var cell in linkCacheProvider.LinkCache.EnumerateAllCells(worldspace.FormKey)) {
                if (cell.Grid is null) continue;

                var gridPoint = cell.Grid.Point;
                if (gridPoint.X != GridXValue || gridPoint.Y != GridYValue) continue;

                if (!ExteriorCellsProvider.RecordCache.TryGetValue(cell.FormKey, out var referencedRecord)) return;
                if (referencedRecord is not IReferencedRecord<ICellGetter> referencedCell) return;

                RecordListVM.SelectedRecord = null;
                RecordListVM.SelectedRecord = referencedCell;
                break;
            }
        });

        IRecordContextMenuProvider CreateContextMenuProvider(IObservable<IMajorRecordGetter?> selectedRecords)
            => cellContextMenuProviderFactory(selectedRecords!.OfType<ICellGetter?>(), this);
    }

    public void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadExteriorCell(ExteriorCellsProvider.WorldspaceFormKey, cell);
    }
}
