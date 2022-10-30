using CreationEditor.GUI.ViewModels.Mod;
namespace CreationEditor.GUI.Views.Mod; 

public partial class ModSelectionWindow {
    public ModSelectionWindow(ModSelectionVM modSelectionVM) {
        InitializeComponent();

        DataContext = modSelectionVM;
    }
}