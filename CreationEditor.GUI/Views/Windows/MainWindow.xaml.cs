using CreationEditor.GUI.ViewModels;
namespace CreationEditor.GUI.Views.Windows; 

public partial class MainWindow {
    public MainWindow() {
        App.UpdateTheme(this);
        InitializeComponent();
        
        DataContext = MainVM.Instance;
    }
}
