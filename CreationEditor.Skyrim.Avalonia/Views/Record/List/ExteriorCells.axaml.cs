using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.List; 

public partial class ExteriorCells : ReactiveUserControl<ExteriorCellsProvider> {
    public ExteriorCells() {
        InitializeComponent();
    }
    
    public ExteriorCells(ExteriorCellsProvider exteriorCellsProvider) : this() {
        DataContext = exteriorCellsProvider;
    }
}
