using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModPicker : ReactiveUserControl<ModPickerVM> {
    public ModPicker() {
        InitializeComponent();
    }

    public ModPicker(ModPickerVM vm) : this() {
        DataContext = vm;
    }
}
