using CreationEditor.GUI.ViewModels.Mod;
namespace CreationEditor.GUI.Views.Windows; 

public partial class ModSelectionWindow {
    public ModSelectionWindow(ModSelectionVM modSelectionVM) {
        App.UpdateTheme(this);
        InitializeComponent();

        DataContext = modSelectionVM;
    }
}