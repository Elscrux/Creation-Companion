using CreationEditor.Avalonia.ViewModels.Config;
using ReactiveUI.Avalonia;
namespace CreationEditor.Avalonia.Views.Config;

public partial class ConfigView : ReactiveUserControl<ConfigVM> {
    public ConfigView() {
        InitializeComponent();
    }
}
