using System;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.Services.Record.Actions;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class InteriorCellsVM : ViewModel, ICellLoadStrategy {
    private readonly IViewportRuntimeService _viewportRuntimeService;
    public InteriorCellsProvider InteriorCellsProvider { get; }

    public IRecordListVM RecordListVM { get; }

    public InteriorCellsVM(
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        InteriorCellsProvider interiorCellsProvider,
        IViewportRuntimeService viewportRuntimeService,
        Func<IObservable<ICellGetter?>, ICellLoadStrategy, CellContextMenuProvider> cellContextMenuProviderFactory) {
        _viewportRuntimeService = viewportRuntimeService;
        InteriorCellsProvider = interiorCellsProvider.DisposeWith(this);

        RecordListVM = recordListVMBuilder
            .WithExtraColumns(extraColumnsBuilder.AddRecordType<ICellGetter>())
            .WithContextMenuProviderFactory(CreateContextMenuProvider)
            .BuildWithSource(InteriorCellsProvider)
            .DisposeWith(this);

        IRecordContextMenuProvider CreateContextMenuProvider(IObservable<IMajorRecordGetter?> selectedRecords)
            => cellContextMenuProviderFactory(selectedRecords!.OfType<ICellGetter>(), this);
    }

    public void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadInteriorCell(cell);
    }
}
