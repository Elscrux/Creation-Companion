using BuildStripper.ViewModels;
using CreationEditor.Avalonia.Attached;
using ReactiveUI.Avalonia;
namespace BuildStripper.Views;

public partial class BuildStripperView : ReactiveUserControl<BuildStripperVM> {
    public BuildStripperView() {
        InitializeComponent();
    }

    public BuildStripperView(BuildStripperVM vm) : this() {
        DataContext = vm;
        FeatureFlagsDataGrid[ListShortcuts.AddProperty] = vm.AddFeatureFlagCommand;
        FeatureFlagsDataGrid[ListShortcuts.RemoveProperty] = vm.DeleteFeatureFlagsCommand;
    }
}
