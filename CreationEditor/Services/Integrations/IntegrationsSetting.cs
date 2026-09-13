using CreationEditor.Services.Settings;
namespace CreationEditor.Services.Integrations;

/// <summary>
/// Persisted settings model holding the user's configured integrations.
/// </summary>
public sealed class IntegrationsSetting : ISettingModel {
    public List<RegisteredIntegration> Integrations { get; set; } = [];
}
