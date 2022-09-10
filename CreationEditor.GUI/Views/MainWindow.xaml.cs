using Elscrux.WPF.Extensions;
namespace CreationEditor.GUI.Views;

public partial class MainWindow : IMainWindow {
    public MainWindow() {
        this.UpdateTheme();
        InitializeComponent();
        
        DockingManager.LoadDockState();
    }
}
