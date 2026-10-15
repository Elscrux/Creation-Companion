using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Integrations.Audio;
using CreationEditor.Services.Integrations.Lip;
namespace LipGenerator.Services;

/// <summary>
/// Config definition for the lip file generator. The lip generator, lip fuzer and audio
/// encoder are selected as integrations, each with their own runtime settings.
/// </summary>
public sealed class LipGeneratorConfig : IConfigDefinition {
    public static IntegrationConfigOption LipGeneratorOption { get; } = new(
        "Lip Generator",
        typeof(ILipGenerator)
    );

    public static IntegrationConfigOption FuzGeneratorOption { get; } = new(
        "Lip Fuzer",
        typeof(ILipFuzer)
    );

    /// <summary>Optional; when not selected the WAV files are kept uncompressed</summary>
    public static IntegrationConfigOption AudioEncoderOption { get; } = new(
        "Audio Encoder",
        typeof(IAudioEncoder),
        " If no audio encoder is selected, the available original WAV files will be used to fuze lips files without additional compression",
        isOptional: true
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        LipGeneratorOption,
        FuzGeneratorOption,
        AudioEncoderOption,
    ];
}
