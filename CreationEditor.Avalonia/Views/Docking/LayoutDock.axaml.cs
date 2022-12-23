using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking; 

public partial class LayoutDock : ReactiveUserControl<LayoutDockVM> {
    public LayoutDock() {
        InitializeComponent();
    }

    public LayoutDock(IDockableVM vm) : this() {
        DataContext = vm;
    }
}

