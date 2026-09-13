using CreationEditor.Avalonia.ViewModels.Integrations;
using FluentAvalonia.UI.Windowing;
namespace CreationEditor.Avalonia.Views.Integrations;

public partial class IntegrationsWindow : FAAppWindow {
    public IntegrationsWindow() {
        InitializeComponent();
    }

    public IntegrationsWindow(IntegrationsVM vm) : this() {
        DataContext = vm;
    }
}
