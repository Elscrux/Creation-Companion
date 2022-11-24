using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
namespace CreationEditor.WPF.Views;

public partial class MainWindow : Window, IMainWindow {
    // public IUniDockService DockingManager => this.FindControl<RootDockGroup>("DockingManager").TheDockManager;

    public MainWindow() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
