using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using Mutagen.Bethesda.Strings;
using NAudio.Wave;
using Serilog;
namespace CreationEditor.Services.Integrations.Lip;

public sealed class FaceFxWrapperRegistrationConfig : IConfigDefinition {
    public static FileConfigOption ToolPathOption { get; } = new(
        "FaceFXWrapper Path",
        "Path to the FaceFXWrapper executable",
        autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Game, @"Tools\Audio\FaceFXWrapper.exe")]);

    public static FileConfigOption FonixDataOption { get; } = new(
        "FonixData Path",
        @"Path to the FonixData.cdf phoneme data file, usually found under Data\Sound\Voice\Processing\FonixData.cdf",
        autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Data, @"Sound\Voice\Processing\FonixData.cdf")]);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ToolPathOption,
        FonixDataOption,
    ];
}

public sealed class FaceFxWrapperRuntimeConfig : IConfigDefinition {
    public static EnumConfigOption LanguageOption { get; } = new(
        "Language",
        typeof(Language),
        "Language used for lip sync generation",
        Language.English);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        LanguageOption,
    ];
}

/// <summary>
/// Integration for the FaceFXWrapper tool, to generate lip files.
/// Calls https://github.com/Nukem9/FaceFXWrapper
/// </summary>
public sealed class FaceFxWrapper(IFileSystem fileSystem, ILogger logger, ConfigValues registrationConfigValues) : ILipGenerator {
    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("13776781-885a-49ff-b72b-cb933c4017e8"),
        "FaceFX Wrapper",
        "A tool for generating Skyrim lip sync files from WAV files and voice text.",
        "Lip Generator",
        typeof(ILipGenerator),
        new FaceFxWrapperRegistrationConfig(),
        new FaceFxWrapperRuntimeConfig(),
        []
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public IReadOnlyList<string> SupportedAudioExtensions { get; } = [".wav"];

    public async Task<IntegrationRunResult> GenerateLip(
        string wavPath,
        string text,
        ConfigValues config,
        CancellationToken token) {
        var toolPath = registrationConfigValues.Get(FaceFxWrapperRegistrationConfig.ToolPathOption);
        var fonixDataPath = registrationConfigValues.Get(FaceFxWrapperRegistrationConfig.FonixDataOption);
        var language = config.Get<Language>(FaceFxWrapperRuntimeConfig.LanguageOption);

        if (string.IsNullOrEmpty(toolPath) || !fileSystem.File.Exists(toolPath)) {
            return new IntegrationRunResult(false, $"FaceFXWrapper executable not found at: {toolPath}");
        }
        if (string.IsNullOrEmpty(fonixDataPath) || !fileSystem.File.Exists(fonixDataPath)) {
            return new IntegrationRunResult(false, $"FonixData.cdf not found at: {fonixDataPath}");
        }

        var lipPath = fileSystem.Path.ChangeExtension(wavPath, ".lip");

        await using var waveFileReader = new WaveFileReader(wavPath);
        var totalTime = waveFileReader.TotalTime;
        if (totalTime.TotalSeconds < 0.5f) {
            logger.Here().Debug("Skipping lip generation for {WavPath} because duration is less than 0.5 seconds", wavPath);
            return new IntegrationRunResult(false, $"Skipped lip generation for {wavPath} because the audio is shorter than 0.5 seconds.");
        }

        // Remove quotes from text to avoid issues with command line arguments
        text = text.Replace("\"", string.Empty);

        var resampledPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
        var arguments = $"\"Skyrim\" \"{ConvertLanguage(language)}\" \"{fonixDataPath}\" \"{wavPath}\" \"{resampledPath}\" \"{lipPath}\" \"{text}\"";

        try {
            var (exitCode, output, error) = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (exitCode != 0) {
                logger.Here().Warning("FaceFxWrapper failed to generate lip file for {WavPath}:\nOutput: {Output}\nError: {Error}", wavPath, output, error);
                return new IntegrationRunResult(false, $"FaceFXWrapper failed to generate lip file for {wavPath}: {error}");
            }

            logger.Here().Debug("Generated lip for {WavPath} with text: {Text}", wavPath, text);
            return new IntegrationRunResult(true, $"Generated lip file for {wavPath}.");
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Lip generation was cancelled: {WavPath}", wavPath);
            return new IntegrationRunResult(false, $"Lip generation for {wavPath} was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to generate lip file: {WavPath}", wavPath);
            return new IntegrationRunResult(false, $"Failed to generate lip file for {wavPath}: {e.Message}");
        }
    }

    private static string ConvertLanguage(Language language) {
        return language switch {
            Language.English => "USEnglish",
            _ => language.ToString(),
        };
    }
}
