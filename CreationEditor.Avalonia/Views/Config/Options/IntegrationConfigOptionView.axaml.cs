using ReactiveUI.Avalonia;
using IntegrationConfigOptionVM = CreationEditor.Avalonia.ViewModels.Config.Options.IntegrationConfigOptionVM;
namespace CreationEditor.Avalonia.Views.Config.Options;

public partial class IntegrationConfigOptionView : ReactiveUserControl<IntegrationConfigOptionVM> {
    public IntegrationConfigOptionView() {
        InitializeComponent();
    }
}
