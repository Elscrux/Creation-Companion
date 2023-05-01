using System.Reactive;
using Autofac;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Skyrim.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class ExteriorCellsVM : ViewModel {
    public ExteriorCellsProvider ExteriorCellsProvider { get; }

    [Reactive] public int GridXValue { get; set; }
    [Reactive] public int GridYValue { get; set; }

    public RecordList ExteriorList { get; }

    public ReactiveCommand<Unit, Unit> SelectGridCell { get; }

    public ExteriorCellsVM(
        ILifetimeScope lifetimeScope,
        IExtraColumnsBuilder extraColumnsBuilder,
        ExteriorCellsProvider exteriorCellsProvider) {
        ExteriorCellsProvider = exteriorCellsProvider;

        var columns = extraColumnsBuilder
            .AddRecordType<ICellGetter>()
            .AddColumnType<CellGridExtraColumns>()
            .Build();

        var newScope = lifetimeScope.BeginLifetimeScope();
        var recordListVM = newScope.Resolve<IRecordListVM>(TypedParameter.From<IRecordProvider>(ExteriorCellsProvider));
        newScope.DisposeWith(recordListVM);

        ExteriorList = new RecordList(columns) { DataContext = recordListVM };

        SelectGridCell = ReactiveCommand.Create(() => {
            if (exteriorCellsProvider.RecordBrowserSettingsVM.LinkCache.TryResolve<IWorldspaceGetter>(ExteriorCellsProvider.WorldspaceFormKey, out var worldspace)) {
                foreach (var cell in worldspace.EnumerateCells()) {
                    if (cell.Grid == null) continue;

                    var gridPoint = cell.Grid.Point;
                    if (gridPoint.X != GridXValue || gridPoint.Y != GridYValue) continue;

                    ExteriorList.ScrollToItem(cell);
                    break;
                }
            }
        });
    }
}
