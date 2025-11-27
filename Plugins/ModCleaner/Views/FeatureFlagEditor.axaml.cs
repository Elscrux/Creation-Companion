using Avalonia.ReactiveUI;
using ModCleaner.ViewModels;
namespace ModCleaner.Views;

public partial class FeatureFlagEditor : ReactiveUserControl<FeatureFlagEditorVM> {
    public FeatureFlagEditor() {
        InitializeComponent();
    }

    public FeatureFlagEditor(FeatureFlagEditorVM vm) : this() {
        ViewModel = vm;
    }
}
