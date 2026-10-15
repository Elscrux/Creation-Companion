namespace CreationEditor.Services.Integrations.Audio;

/// <summary>
/// Integration capability for encoding/decoding audio files into a compressed game audio format.
/// Concrete tools that can encode or decode audio implement this interface.
/// </summary>
public interface IAudioEncoder : IIntegration {
    IReadOnlyList<string> SupportedEncodedExtensions { get; }
    IReadOnlyList<string> SupportedUnencodedExtensions { get; }

    /// <summary>Encodes the given PCM/WAV file into the encoder's audio format</summary>
    /// <param name="sourcePath">Path of the source PCM/WAV file</param>
    /// <param name="encodedPath">Path of the encoded audio file to create</param>
    /// <param name="config">Runtime configuration for the encoding</param>
    /// <param name="token">Cancellation token</param>
    Task<IntegrationRunResult> Encode(
        string sourcePath,
        string encodedPath,
        ConfigValues config,
        CancellationToken token);

    /// <summary>Decodes the given encoded audio file into a PCM/WAV file</summary>
    /// <param name="encodedPath">Path of the encoded audio file</param>
    /// <param name="targetPath">Path of the decoded PCM/WAV file to create</param>
    /// <param name="config">Runtime configuration for the decoding</param>
    /// <param name="token">Cancellation token</param>
    Task<IntegrationRunResult> Decode(
        string encodedPath,
        string targetPath,
        ConfigValues config,
        CancellationToken token);
}
