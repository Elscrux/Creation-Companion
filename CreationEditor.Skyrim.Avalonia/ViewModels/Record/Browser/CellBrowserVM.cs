using System;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.Views.Record.List;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser; 

public interface ICellBrowserVM {
    public InteriorCellsVM InteriorCellsVM { get; }
    public ExteriorCellsVM ExteriorCellsVM { get; }
    public PlacedListVM PlacedListVM { get; }
    
    public InteriorCells InteriorCells { get; }
    public ExteriorCells ExteriorCells { get; }
    public PlacedList PlacedList { get; }

    public int SelectedTab { get; set; }
    
    public bool ShowReferences { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleReferences { get; }
}

public sealed class CellBrowserVM : ViewModel, ICellBrowserVM {
    public InteriorCellsVM InteriorCellsVM { get; }
    public ExteriorCellsVM ExteriorCellsVM { get; }
    public PlacedListVM PlacedListVM { get; }

    public InteriorCells InteriorCells { get; } = new();
    public ExteriorCells ExteriorCells { get; } = new();
    public PlacedList PlacedList { get; } = new();
    
    [Reactive] public int SelectedTab { get; set; }
    [Reactive] public bool ShowReferences { get; set; }
    
    public ReactiveCommand<Unit, Unit> ToggleReferences { get; }

    public CellBrowserVM(
        InteriorCellsVM interiorCellsVM,
        ExteriorCellsVM exteriorCellsVM,
        PlacedListVM placedListVM) {
        InteriorCells.DataContext = InteriorCellsVM = interiorCellsVM;
        ExteriorCells.DataContext = ExteriorCellsVM = exteriorCellsVM;
        PlacedList.DataContext = PlacedListVM = placedListVM;
        
        ToggleReferences = ReactiveCommand.Create(() => { ShowReferences = !ShowReferences; });

        this.WhenAnyValue(
                x => x.InteriorCellsVM.InteriorCellsProvider.SelectedRecord,
                x => x.ExteriorCellsVM.ExteriorCellsProvider.SelectedRecord,
                (interiorCell, exteriorCell)
                    => SelectedTab == 0 ? interiorCell : exteriorCell)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Subscribe(cell => PlacedListVM.PlacedProvider.Cell = cell?.Record)
            .DisposeWith(this);
    }
}
