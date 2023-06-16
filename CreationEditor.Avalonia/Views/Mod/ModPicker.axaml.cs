using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod; 

public partial class ModPicker : UserControl {
    public ModPicker() {
        InitializeComponent();
    }

    public ModPicker(ModPickerVM vm) : this() {
        DataContext = vm;
    }
}

