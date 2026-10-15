namespace CreationEditor.Services.Integrations.Lip;

/// <summary>
/// Integration capability for generating lip-sync animation files. Concrete tools that can
/// generate lip files implement this interface.
/// </summary>
public interface ILipGenerator : IIntegration {
    IReadOnlyList<string> SupportedAudioExtensions { get; }

    /// <summary>Generates a lip file for the given WAV file and text</summary>
    Task<IntegrationRunResult> GenerateLip(
        string wavPath,
        string text,
        ConfigValues config,
        CancellationToken token);
}
