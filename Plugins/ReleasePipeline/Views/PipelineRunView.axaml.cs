using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class PipelineRunView : ReactiveUserControl<PipelineRunVM> {
    public PipelineRunView() {
        InitializeComponent();
    }

    public PipelineRunView(PipelineRunVM vm) : this() {
        DataContext = vm;
    }
}
