using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.List; 

public partial class InteriorCells : ReactiveUserControl<InteriorCellsProvider> {
    public InteriorCells() {
        InitializeComponent();
    }
    
    public InteriorCells(InteriorCellsProvider interiorCellsProvider) : this() {
        DataContext = interiorCellsProvider;
    }
}
