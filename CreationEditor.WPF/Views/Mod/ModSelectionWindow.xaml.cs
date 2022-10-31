using CreationEditor.WPF.ViewModels.Mod;
namespace CreationEditor.WPF.Views.Mod; 

public partial class ModSelectionWindow {
    public ModSelectionWindow(ModSelectionVM modSelectionVM) {
        InitializeComponent();

        DataContext = modSelectionVM;
    }
}