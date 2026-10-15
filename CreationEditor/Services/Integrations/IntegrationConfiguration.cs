namespace CreationEditor.Services.Integrations;

public sealed class IntegrationConfiguration {
    public Guid IntegrationId { get; set; }

    public ConfigValues ConfigValues { get; set; } = new();
}
