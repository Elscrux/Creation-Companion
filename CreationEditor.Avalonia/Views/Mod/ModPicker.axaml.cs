using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModPicker : ReactiveUserControl<IModPickerVM> {
    public ModPicker() {
        InitializeComponent();
    }

    public ModPicker(IModPickerVM vm) : this() {
        DataContext = vm;
    }
}
