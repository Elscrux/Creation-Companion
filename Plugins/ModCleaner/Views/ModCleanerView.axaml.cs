using CreationEditor.Avalonia.Attached;
using ModCleaner.ViewModels;
using ReactiveUI.Avalonia;
namespace ModCleaner.Views;

public partial class ModCleanerView : ReactiveUserControl<ModCleanerVM> {
    public ModCleanerView() {
        InitializeComponent();
    }

    public ModCleanerView(ModCleanerVM vm) : this() {
        DataContext = vm;
        FeatureFlagsDataGrid[ListShortcuts.AddProperty] = vm.AddFeatureFlagCommand;
        FeatureFlagsDataGrid[ListShortcuts.RemoveProperty] = vm.DeleteFeatureFlagsCommand;
    }
}
