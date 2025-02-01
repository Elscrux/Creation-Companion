using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;

public sealed partial class CellBrowserVM : ViewModel, ICellBrowserVM {
    public InteriorCellsVM InteriorCellsVM { get; }
    public ExteriorCellsVM ExteriorCellsVM { get; }
    public PlacedListVM PlacedListVM { get; }

    [Reactive] public partial int SelectedTab { get; set; }
    [Reactive] public partial bool ShowPlaced { get; set; }

    public ReactiveCommand<Unit, Unit> TogglePlaced { get; }

    public CellBrowserVM(
        InteriorCellsVM interiorCellsVM,
        ExteriorCellsVM exteriorCellsVM,
        PlacedListVM placedListVM) {
        InteriorCellsVM = interiorCellsVM.DisposeWith(this);
        ExteriorCellsVM = exteriorCellsVM.DisposeWith(this);
        PlacedListVM = placedListVM.DisposeWith(this);

        TogglePlaced = ReactiveCommand.Create(() => { ShowPlaced = !ShowPlaced; });

        this.WhenAnyValue(x => x.PlacedListVM.PlacedProvider.CellFormKey)
            .Where(x => x != FormKey.Null)
            .Take(1)
            .Subscribe(_ => ShowPlaced = true)
            .DisposeWith(this);

        this.WhenAnyValue(
                x => x.InteriorCellsVM.RecordListVM.SelectedRecord,
                x => x.ExteriorCellsVM.RecordListVM.SelectedRecord,
                (interiorCell, exteriorCell)
                    => SelectedTab == 0 ? interiorCell : exteriorCell)
            .ThrottleMedium()
            .Subscribe(cell => PlacedListVM.PlacedProvider.CellFormKey = cell?.Record.FormKey ?? FormKey.Null)
            .DisposeWith(this);
    }
}
