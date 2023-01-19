using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.List; 

public partial class PlacedList : ReactiveUserControl<IPlacedListVM> {
    public PlacedList() {
        InitializeComponent();
    }
    
    public PlacedList(IPlacedListVM cellListVM) : this() {
        DataContext = cellListVM;
    }
}
