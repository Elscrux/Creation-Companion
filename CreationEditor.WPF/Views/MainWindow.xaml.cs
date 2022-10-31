using AvalonDock;
using MahApps.Metro.Controls.Dialogs;
namespace CreationEditor.WPF.Views;

public partial class MainWindow : IMainWindow {
    public DockingManager DockingManager => MainDockingManager;
    
    public MainWindow() {
        InitializeComponent();

        Closing += OnClosing;
    }
    
    private bool _closeMe;
    private async void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e) {
        if (e.Cancel) return;

        e.Cancel = !_closeMe;
        if (_closeMe) return;

        var mySettings = new MetroDialogSettings {
            AffirmativeButtonText = "Quit",
            NegativeButtonText = "Cancel",
            AnimateShow = true,
            AnimateHide = false
        };
        var result = await this.ShowMessageAsync(
            "Quit?",
            "Are you sure you want to quit the program?",
            MessageDialogStyle.AffirmativeAndNegative, mySettings);
        _closeMe = result == MessageDialogResult.Affirmative;

        if (_closeMe) Close();
    }
}
