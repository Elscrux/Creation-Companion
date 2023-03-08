using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.List;

public partial class PlacedList : ReactiveUserControl<PlacedListVM> {
    public PlacedList() {
        InitializeComponent();
    }

    public PlacedList(PlacedListVM placedListVM) : this() {
        DataContext = placedListVM;
    }
}
