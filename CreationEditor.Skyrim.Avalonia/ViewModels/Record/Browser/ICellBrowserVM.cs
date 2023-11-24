using System.Reactive;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;

public interface ICellBrowserVM : IDisposableDropoff {
    public InteriorCellsVM InteriorCellsVM { get; }
    public ExteriorCellsVM ExteriorCellsVM { get; }
    public PlacedListVM PlacedListVM { get; }

    public int SelectedTab { get; set; }

    public bool ShowPlaced { get; set; }
    public ReactiveCommand<Unit, Unit> TogglePlaced { get; }
}
