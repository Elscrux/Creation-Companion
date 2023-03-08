using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class DockingManager : ReactiveUserControl<DockingManagerVM> {
    public DockingManager() {
        InitializeComponent();
    }

    public DockingManager(DockingManagerVM vm) : this() {
        DataContext = vm;
    }
}
