using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.List;

public partial class ExteriorCells : ReactiveUserControl<ExteriorCellsVM> {
    public ExteriorCells() {
        InitializeComponent();
    }

    public ExteriorCells(ExteriorCellsVM vm) : this() {
        DataContext = vm;
    }
}
