using Avalonia.ReactiveUI;
using MapperPlugin.ViewModels;
namespace MapperPlugin.Views;

public partial class MapperView : ReactiveUserControl<MapperVM> {
    public MapperView() {
        InitializeComponent();
    }

    public MapperView(MapperVM vm) : this() {
        DataContext = vm;
    }
}
