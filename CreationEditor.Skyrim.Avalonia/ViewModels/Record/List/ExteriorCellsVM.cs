using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Avalonia.Resources.Comparer;
using CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;
using CreationEditor.Skyrim.Extension;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public class ExteriorCellsVM : CellListVM {
    [Reactive] public FormKey WorldspaceFormKey { get; set; }

    [Reactive] public int GridXValue { get; set; }
    [Reactive] public int GridYValue { get; set; }

    [Reactive] public bool ShowWildernessCells { get; set; } = true;

    public RecordList ExteriorList { get; }

    public ReactiveCommand<Unit, Unit> SelectGridCell { get; }
    public ReactiveCommand<Unit, Unit> ToggleWildernessCells { get; }

    public ExteriorCellsVM(
        IExtraColumnProvider extraColumnProvider,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceQuery referenceQuery,
        IRecordController recordController,
        IDockFactory dockFactory,
        IViewportRuntimeService viewportRuntimeService,
        IRecordEditorController recordEditorController)
        : base(recordListFactory, recordBrowserSettingsVM, referenceQuery, recordController, dockFactory, viewportRuntimeService, recordEditorController) {
        
        CustomFilter = record => ShowWildernessCells || !record.Record.EditorID.IsNullOrEmpty();
        
        ExteriorList = new RecordList(extraColumnProvider.GetColumns(typeof(ICellGetter))) { DataContext = this };

        ExteriorList.InsertColumn(3, new DataGridTextColumn {
            Header = "Grid",
            Binding = new Binding("Record.Grid.Point", BindingMode.OneWay),
            CanUserSort = true,
            CustomSortComparer = SkyrimRecordComparers.CellGridComparer,
            Width = new DataGridLength(100),
        });

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache, x => x.WorldspaceFormKey)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .DoOnGuiAndSwitchBack(_ => IsBusy = true)
            .Subscribe(_ => {
                if (RecordBrowserSettingsVM.LinkCache.TryResolve<IWorldspaceGetter>(WorldspaceFormKey, out var worldspace)) {
                    RecordCache.Clear();
                    RecordCache.Edit(updater => {
                        foreach (var cell in worldspace.EnumerateCells()) {
                            var formLinks = ReferenceQuery.GetReferences(cell.FormKey, RecordBrowserSettingsVM.LinkCache);
                            var referencedRecord = new ReferencedRecord<Cell, ICellGetter>(cell, formLinks);
                            updater.AddOrUpdate(referencedRecord);
                        }
                    });
                }

                Dispatcher.UIThread.Post(() => IsBusy = false);
            })
            .DisposeWith(this);

        SelectGridCell = ReactiveCommand.Create(() => {
            if (RecordBrowserSettingsVM.LinkCache.TryResolve<IWorldspaceGetter>(WorldspaceFormKey, out var worldspace)) {
                foreach (var cell in worldspace.EnumerateCells()) {
                    if (cell.Grid == null) continue;

                    var gridPoint = cell.Grid.Point;
                    if (gridPoint.X != GridXValue || gridPoint.Y != GridYValue) continue;

                    ExteriorList.ScrollToItem(cell);
                    break;
                }
            }
        });

        ToggleWildernessCells = ReactiveCommand.Create(RecordBrowserSettingsVM.RequestUpdate);
    }

}