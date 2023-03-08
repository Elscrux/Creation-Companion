using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Setting;
namespace CreationEditor.Avalonia.Views.Setting;

public partial class SettingsView : ReactiveUserControl<ISettingsVM> {
    public SettingsView() {
        InitializeComponent();
    }

    public SettingsView(ISettingsVM settingsVM) : this() {
        DataContext = settingsVM;
    }
}
