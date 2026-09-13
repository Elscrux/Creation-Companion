using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class ChecksStepView : ReactiveUserControl<ChecksStepVM> {
    public ChecksStepView() {
        InitializeComponent();
    }

    public ChecksStepView(ChecksStepVM vm) : this() {
        DataContext = vm;
    }
}
