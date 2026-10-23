using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class PipelineWizard : ReactiveUserControl<ReleasePipelineWizardVM> {
    public PipelineWizard() {
        InitializeComponent();
    }

    public PipelineWizard(ReleasePipelineWizardVM vm) : this() => DataContext = vm;
}
