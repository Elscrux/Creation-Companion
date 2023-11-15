using System;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.Views.Record.List;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;

public sealed class CellBrowserVM : ViewModel, ICellBrowserVM {
    public InteriorCellsVM InteriorCellsVM { get; }
    public ExteriorCellsVM ExteriorCellsVM { get; }
    public PlacedListVM PlacedListVM { get; }

    public InteriorCells InteriorCells { get; } = new();
    public ExteriorCells ExteriorCells { get; } = new();
    public PlacedList PlacedList { get; } = new();

    [Reactive] public int SelectedTab { get; set; }
    [Reactive] public bool ShowPlaced { get; set; }

    public ReactiveCommand<Unit, Unit> TogglePlaced { get; }

    public CellBrowserVM(
        InteriorCellsVM interiorCellsVM,
        ExteriorCellsVM exteriorCellsVM,
        PlacedListVM placedListVM) {
        InteriorCells.DataContext = InteriorCellsVM = interiorCellsVM;
        ExteriorCells.DataContext = ExteriorCellsVM = exteriorCellsVM;
        PlacedList.DataContext = PlacedListVM = placedListVM;

        TogglePlaced = ReactiveCommand.Create(() => { ShowPlaced = !ShowPlaced; });

        this.WhenAnyValue(x => x.PlacedListVM.PlacedProvider.CellFormKey)
            .Where(x => x != FormKey.Null)
            .Take(1)
            .Subscribe(_ => ShowPlaced = true)
            .DisposeWith(this);

        this.WhenAnyValue(
                x => x.InteriorCellsVM.InteriorCellsProvider.SelectedRecord,
                x => x.ExteriorCellsVM.ExteriorCellsProvider.SelectedRecord,
                (interiorCell, exteriorCell)
                    => SelectedTab == 0 ? interiorCell : exteriorCell)
            .ThrottleMedium()
            .Subscribe(cell => PlacedListVM.PlacedProvider.CellFormKey = cell?.Record.FormKey ?? FormKey.Null)
            .DisposeWith(this);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);

        InteriorCellsVM.Dispose();
        ExteriorCellsVM.Dispose();
        PlacedListVM.Dispose();
    }
}
