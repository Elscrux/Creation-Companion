using Avalonia.ReactiveUI;
using ModCleaner.ViewModels;
namespace ModCleaner.Views;

public partial class ModCleanerView : ReactiveUserControl<ModCleanerVM> {
    public ModCleanerView() {
        InitializeComponent();
    }

    public ModCleanerView(ModCleanerVM vm) : this() {
        DataContext = vm;
    }
}
