using Avalonia;
using Avalonia.Markup.Xaml;
using CreationEditor.WPF.ViewModels.Mod;
using FluentAvalonia.UI.Windowing;
namespace CreationEditor.WPF.Views.Mod;

public partial class ModSelectionWindow : AppWindow {
    public ModSelectionWindow() {
        InitializeComponent();
    }

    public ModSelectionWindow(ModSelectionVM modSelectionVM) {
        InitializeComponent();

        DataContext = modSelectionVM;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
