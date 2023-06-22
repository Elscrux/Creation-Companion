using Avalonia.ReactiveUI;
namespace VanillaDuplicateCleaner;

public partial class VanillaDuplicateCleanerView : ReactiveUserControl<VanillaDuplicateCleanerVM> {
    public VanillaDuplicateCleanerView() {
        InitializeComponent();
    }

    public VanillaDuplicateCleanerView(VanillaDuplicateCleanerVM vm) : this() {
        DataContext = vm;
    }
}
