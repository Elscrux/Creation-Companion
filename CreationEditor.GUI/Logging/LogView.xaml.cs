using System.Windows;
namespace CreationEditor.GUI.Logging; 

public partial class LogView {
    public LogView() {
        InitializeComponent();

        DataContext = new Log();
    }

    private void Clear_OnExecuted(object sender, RoutedEventArgs routedEventArgs) => Log.Clear();
}
