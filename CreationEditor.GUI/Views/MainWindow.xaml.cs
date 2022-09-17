using Elscrux.WPF.Extensions;
using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI.Views;

public partial class MainWindow : IMainWindow {
    public MainWindow() {
        this.UpdateTheme();
        InitializeComponent();

        DockingManager.SetDockingManager(this, MainDockingManager);
    }
}
