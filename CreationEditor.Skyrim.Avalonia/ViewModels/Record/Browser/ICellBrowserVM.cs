using System.Reactive;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.Views.Record.List;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;

public interface ICellBrowserVM : IDisposableDropoff {
    public InteriorCellsVM InteriorCellsVM { get; }
    public ExteriorCellsVM ExteriorCellsVM { get; }
    public PlacedListVM PlacedListVM { get; }

    public InteriorCells InteriorCells { get; }
    public ExteriorCells ExteriorCells { get; }
    public PlacedList PlacedList { get; }

    public int SelectedTab { get; set; }

    public bool OpenReferences { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleReferences { get; }
}