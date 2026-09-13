using CreationEditor.Services.Integrations;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// Wraps a configured integration with its resolved implementation, exposing display info for
/// the Integrations window cards.
/// </summary>
public sealed partial class IntegrationItemVM : ViewModel {
    public RegisteredIntegration Integration { get; }
    public IIntegration Implementation { get; }

    public string DisplayName => Implementation.DisplayName;
    public string Description => Implementation.Description;
    public string InstallPath => Integration.InstallPath;

    public IntegrationItemVM(RegisteredIntegration integration, IIntegration implementation) {
        Integration = integration;
        Implementation = implementation;
    }
}
