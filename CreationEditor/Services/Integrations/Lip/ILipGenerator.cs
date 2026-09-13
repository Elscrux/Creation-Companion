namespace CreationEditor.Services.Integrations.Lip;

/// <summary>
/// Integration capability for generating lip-sync animation files. Concrete tools that can
/// generate lip files implement this interface.
/// </summary>
public interface ILipGenerator : IIntegration {
    /// <summary>Generates a lip file for the given WAV and text.</summary>
    Task<IntegrationRunResult> GenerateLipAsync(
        string wavPath,
        string text,
        object config,
        CancellationToken token);
}
