namespace CreationEditor.GUI.Views.Windows; 

public interface IMainWindow {
    public object DataContext { get; set; }
}

public partial class MainWindow : IMainWindow {
    public MainWindow() {
        App.UpdateTheme(this);
        InitializeComponent();
        
        DockingManager.LoadDockState();
    }
}
