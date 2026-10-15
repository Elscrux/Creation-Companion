namespace CreationEditor.Services.Integrations;

/// <summary>
/// A registered integration instance: a chosen <see cref="IIntegration"/> paired with its
/// registration settings which need to be configured to set up the integration.
/// </summary>
public sealed class IntegrationRegistration {
    /// <summary>Stable id of the <see cref="IIntegration"/> this instance uses</summary>
    public Guid IntegrationId { get; set; }

    /// <summary>The tool's registration settings</summary>
    public ConfigValues? RegistrationConfig { get; set; }

    /// <summary>Name of the source this integration was installed/updated from, or null if installed manually</summary>
    public string? SourceName { get; set; }

    /// <summary>The installed version, used to compare against a source's latest version for updates</summary>
    public string? InstalledVersion { get; set; }
}
