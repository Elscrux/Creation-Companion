using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class DataSourceStepView : ReactiveUserControl<DataSourceStepVM> {
    public DataSourceStepView() {
        InitializeComponent();
    }

    public DataSourceStepView(DataSourceStepVM vm) : this() {
        DataContext = vm;
    }
}
