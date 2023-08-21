using Avalonia.ReactiveUI;
using VanillaDuplicateCleaner.ViewModels;
namespace VanillaDuplicateCleaner.Views;

public partial class VanillaDuplicateCleanerView : ReactiveUserControl<VanillaDuplicateCleanerVM> {
    public VanillaDuplicateCleanerView() {
        InitializeComponent();
    }

    public VanillaDuplicateCleanerView(VanillaDuplicateCleanerVM vm) : this() {
        DataContext = vm;
    }
}
