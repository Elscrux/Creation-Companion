using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.List;

public partial class InteriorCells : ReactiveUserControl<InteriorCellsVM> {
    public InteriorCells() {
        InitializeComponent();
    }

    public InteriorCells(InteriorCellsVM vm) : this() {
        DataContext = vm;
    }
}
