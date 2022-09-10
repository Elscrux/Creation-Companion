using CreationEditor.GUI.ViewModels.Mod;
using Elscrux.WPF.Extensions;
namespace CreationEditor.GUI.Views.Mod; 

public partial class ModSelectionWindow {
    public ModSelectionWindow(ModSelectionVM modSelectionVM) {
        this.UpdateTheme();
        InitializeComponent();

        DataContext = modSelectionVM;
    }
}