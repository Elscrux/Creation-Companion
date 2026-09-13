namespace CreationEditor.Services.Integrations;

/// <summary>
/// A registered integration instance: a chosen <see cref="IIntegration"/> paired with its
/// install location (the fundamental setup required for the tool to work) and a persisted
/// configuration (runtime parameters used when the integration runs). This is what the user adds
/// via the Integrations window.
/// </summary>
public sealed class RegisteredIntegration {
    /// <summary>Stable id of the <see cref="IIntegration"/> this instance uses.</summary>
    public Guid ImplementationId { get; set; }

    /// <summary>Path to the installed tool executable (required setup).</summary>
    public string InstallPath { get; set; } = string.Empty;

    /// <summary>The tool's configuration model (runtime parameters, serialized polymorphically).</summary>
    public object? Config { get; set; }
}
