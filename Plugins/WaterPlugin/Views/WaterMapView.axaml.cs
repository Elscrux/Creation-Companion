using Avalonia.ReactiveUI;
using WaterPlugin.ViewModels;
namespace WaterPlugin.Views;

public partial class WaterMapView : ReactiveUserControl<WaterMapVM> {
    public WaterMapView() {
        InitializeComponent();
    }

    public WaterMapView(WaterMapVM vm) : this() {
        DataContext = vm;
    }
}
