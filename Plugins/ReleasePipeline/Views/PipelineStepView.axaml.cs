using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class PipelineStepView : ReactiveUserControl<PipelineSelectionStepVM> {
    public PipelineStepView() {
        InitializeComponent();
    }

    public PipelineStepView(PipelineSelectionStepVM vm) : this() => DataContext = vm;
}
