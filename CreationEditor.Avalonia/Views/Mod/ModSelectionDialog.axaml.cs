using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModSelectionPopup : ReactiveUserControl<ModSelectionVM> {
    public ModSelectionPopup() {
        InitializeComponent();
    }

    public ModSelectionPopup(ModSelectionVM modSelectionVM) : this() {
        DataContext = modSelectionVM;
    }

    public void Show() => IsVisible = true;
    public void Hide() {
        IsVisible = false;
        CancelButton.IsEnabled = true;
    }

    public void LoadAndHide() {
        ViewModel?.LoadMods();
        Hide();
    }
}
