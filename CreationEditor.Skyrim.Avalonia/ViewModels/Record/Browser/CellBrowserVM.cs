using System;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.Views.Record.List;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser; 

public interface ICellBrowserVM {
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
    public IPlacedListVM PlacedListVM { get; }

    public InteriorCells InteriorCells { get; } = new();
    public ExteriorCells ExteriorCells { get; } = new();
    public PlacedList PlacedList { get; } = new();
    
    [Reactive] public int SelectedTab { get; set; }
    [Reactive] public bool ShowReferences { get; set; }
    
    public ReactiveCommand<Unit, Unit> ToggleReferences { get; }

    public CellBrowserVM(
        InteriorCellsVM interiorCellsVM,
        ExteriorCellsVM exteriorCellsVM,
        IPlacedListVM placedListVM) {
        InteriorCells.DataContext = InteriorCellsVM = interiorCellsVM;
        ExteriorCells.DataContext = ExteriorCellsVM = exteriorCellsVM;
        PlacedList.DataContext = PlacedListVM = placedListVM;
        
        ToggleReferences = ReactiveCommand.Create(() => { ShowReferences = !ShowReferences; });

        this.WhenAnyValue(
                x => x.InteriorCellsVM.SelectedRecord,
                x => x.ExteriorCellsVM.SelectedRecord,
                (interiorCell, exteriorCell)
                    => SelectedTab == 0 ? interiorCell : exteriorCell)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Subscribe(cell => PlacedListVM.Cell = cell?.Record);
    }
}
