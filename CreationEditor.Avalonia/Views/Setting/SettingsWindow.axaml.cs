using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Setting;

namespace CreationEditor.Avalonia.Views.Setting; 

public partial class SettingsWindow : Window {
    public SettingsWindow() {
        InitializeComponent();
    }
    
    public SettingsWindow(ISettingsVM settingsVM) : this() {
        DataContext = settingsVM;
    }
}

