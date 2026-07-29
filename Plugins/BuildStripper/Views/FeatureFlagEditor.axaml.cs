using BuildStripper.ViewModels;
using ReactiveUI.Avalonia;
namespace BuildStripper.Views;

public partial class FeatureFlagEditor : ReactiveUserControl<FeatureFlagEditorVM> {
    public FeatureFlagEditor() {
        InitializeComponent();
    }

    public FeatureFlagEditor(FeatureFlagEditorVM vm) : this() {
        ViewModel = vm;
    }
}
