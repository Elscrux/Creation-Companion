using CreationEditor.Services.Settings;
namespace CreationEditor.Services.Integrations;

/// <summary>
/// Persisted settings model holding the user's registered integrations.
/// </summary>
public sealed class IntegrationsDto : ISettingModel {
    public List<IntegrationRegistration> Integrations { get; set; } = [];
}
