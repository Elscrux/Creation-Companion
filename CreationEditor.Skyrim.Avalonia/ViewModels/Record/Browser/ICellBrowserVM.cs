using System.Reactive;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;

public interface ICellBrowserVM : IDisposableDropoff {
    InteriorCellsVM InteriorCellsVM { get; }
    ExteriorCellsVM ExteriorCellsVM { get; }
    PlacedListVM PlacedListVM { get; }

    int SelectedTab { get; set; }

    bool ShowPlaced { get; set; }
    ReactiveCommand<Unit, Unit> TogglePlaced { get; }
}
