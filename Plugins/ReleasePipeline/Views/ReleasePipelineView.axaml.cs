using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class ReleasePipelineView : ReactiveUserControl<ReleasePipelineWizardVM> {
    public ReleasePipelineView() {
        InitializeComponent();
    }

    public ReleasePipelineView(ReleasePipelineWizardVM vm) : this() {
        DataContext = vm;
    }
}
