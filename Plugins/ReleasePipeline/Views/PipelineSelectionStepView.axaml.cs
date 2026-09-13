using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class PipelineSelectionStepView : ReactiveUserControl<PipelineSelectionStepVM> {
    public PipelineSelectionStepView() {
        InitializeComponent();
    }

    public PipelineSelectionStepView(PipelineSelectionStepVM vm) : this() {
        DataContext = vm;
    }
}
