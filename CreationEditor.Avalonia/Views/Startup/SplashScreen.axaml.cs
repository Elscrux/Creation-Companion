using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;
namespace CreationEditor.Avalonia.Views.Startup;

public partial class SplashScreen : UserControl {
    public SplashScreen() {
        InitializeComponent();
    }

    public SplashScreen(IFAApplicationSplashScreen vm) : this() {
        DataContext = vm;
    }
}
