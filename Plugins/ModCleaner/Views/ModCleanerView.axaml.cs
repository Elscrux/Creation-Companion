using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Attached;
using ModCleaner.ViewModels;
namespace ModCleaner.Views;

public partial class ModCleanerView : ReactiveUserControl<ModCleanerVM> {
    public ModCleanerView() {
        InitializeComponent();
    }

    public ModCleanerView(ModCleanerVM vm) : this() {
        DataContext = vm;
        FeatureFlagsDataGrid[ListShortcuts.AddProperty] = vm.AddFeatureFlag;
        FeatureFlagsDataGrid[ListShortcuts.RemoveProperty] = vm.DeleteFeatureFlags;
    }
}
