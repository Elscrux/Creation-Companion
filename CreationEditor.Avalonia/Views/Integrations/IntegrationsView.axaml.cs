using CreationEditor.Avalonia.ViewModels.Integrations;
using ReactiveUI.Avalonia;
namespace CreationEditor.Avalonia.Views.Integrations;

public partial class IntegrationsView : ReactiveUserControl<IntegrationsVM> {
    public IntegrationsView() {
        InitializeComponent();
    }

    public IntegrationsView(IntegrationsVM vm) : this() {
        DataContext = vm;
    }
}
