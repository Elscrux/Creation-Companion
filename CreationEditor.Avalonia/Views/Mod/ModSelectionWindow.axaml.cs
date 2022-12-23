using CreationEditor.Avalonia.ViewModels.Mod;
using FluentAvalonia.UI.Windowing;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModSelectionWindow : AppWindow {
    public ModSelectionWindow() {
        InitializeComponent();
    }

    public ModSelectionWindow(ModSelectionVM modSelectionVM) : this() {
        DataContext = modSelectionVM;
    }
}
