using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModPickerButton : ReactiveUserControl<IModPickerVM> {
    public ModPickerButton() {
        InitializeComponent();
    }

    public ModPickerButton(IModPickerVM vm) : this() {
        DataContext = vm;
    }
}
