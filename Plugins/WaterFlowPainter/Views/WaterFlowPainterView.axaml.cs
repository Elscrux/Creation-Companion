using ReactiveUI.Avalonia;
using WaterFlowPainter.ViewModels;
namespace WaterFlowPainter.Views;

public partial class WaterFlowPainterView : ReactiveUserControl<WaterFlowPainterVM> {
    public WaterFlowPainterView() {
        InitializeComponent();
    }

    public WaterFlowPainterView(WaterFlowPainterVM viewModel) : this() => DataContext = viewModel;
}
