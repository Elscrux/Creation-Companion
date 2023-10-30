using Avalonia.Controls;
namespace CreationEditor.Avalonia.Views;

public partial class SplashScreen : UserControl {
    public SplashScreen() {
        InitializeComponent();
    }

    public SplashScreen(ISplashScreenVM splashScreenVM) : this() {
        DataContext = splashScreenVM;
    }
}
