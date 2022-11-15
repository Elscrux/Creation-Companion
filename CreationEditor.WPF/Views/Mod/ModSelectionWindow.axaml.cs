using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CreationEditor.WPF.ViewModels.Mod;
namespace CreationEditor.WPF.Views.Mod;

public partial class ModSelectionWindow : Window {
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
