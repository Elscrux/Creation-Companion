﻿using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.Services.Viewport;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
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
        IViewportRuntimeService viewportRuntimeService) {
        _viewportRuntimeService = viewportRuntimeService;
        InteriorCellsProvider = interiorCellsProvider.DisposeWith(this);

        RecordListVM = recordListVMBuilder
            .WithExtraColumns(extraColumnsBuilder.AddRecordType<ICellGetter>())
            .BuildWithSource(InteriorCellsProvider)
            .AddSetting<ICellLoadStrategy>(this)
            .DisposeWith(this);
    }

    public void LoadCell(ICellGetter cell) {
        _viewportRuntimeService.LoadInteriorCell(cell);
    }
}
