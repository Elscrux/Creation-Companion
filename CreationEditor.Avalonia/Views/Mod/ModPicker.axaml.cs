using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModPicker : ReactiveUserControl<IModPickerVM> {
    public ModPicker() {
        InitializeComponent();
    }

    public ModPicker(MultiModPickerVM vm) : this() {
        DataContext = vm;
    }
}
