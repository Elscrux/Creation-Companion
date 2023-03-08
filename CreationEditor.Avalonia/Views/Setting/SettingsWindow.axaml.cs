using CreationEditor.Avalonia.ViewModels.Setting;
using FluentAvalonia.UI.Windowing;

namespace CreationEditor.Avalonia.Views.Setting;

public partial class SettingsWindow : AppWindow {
    public SettingsWindow() {
        InitializeComponent();
    }

    public SettingsWindow(ISettingsVM settingsVM) : this() {
        DataContext = settingsVM;
    }
}
