using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using Mutagen.Bethesda.Strings;
using Serilog;
namespace CreationEditor.Services.Integrations.Lip;

public sealed class LipGeneratorWrapperRegistrationConfig : IConfigDefinition {
    public static FileConfigOption ToolPathOption { get; } = new(
        "LipGenerator Path",
        "Path to the LipGenerator executable",
        autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Game, @"Tools\LipGen\LipGenerator\LipGenerator.exe")]);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ToolPathOption,
    ];
}

public sealed class LipGeneratorWrapperRuntimeConfig : IConfigDefinition {
    public static EnumConfigOption LanguageOption { get; } = new(
        "Language",
        typeof(Language),
        "Language used for lip sync generation",
        Language.English,
        true);

    public static FloatConfigOption GestureExaggerationOption { get; } = new(
        "Gesture Exaggeration",
        "Intensity of facial gestures (0.5 = subtle, 3.0 = very exaggerated)",
        1f,
        0.5f,
        3f,
        0.1f,
        true);

    public static FloatConfigOption LipAnimSpeedOption { get; } = new(
        "Lip Animation Speed",
        "Speed multiplier for lip animations (0.5 = slow, 2.0 = fast)",
        1f,
        0.5f,
        2f,
        0.01f,
        true);

    public static FloatConfigOption LipAnimDelayOption { get; } = new(
        "Lip Animation Delay",
        "Delay in seconds before lip animations start",
        0f,
        0f,
        2f,
        0.01f,
        true);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        LanguageOption,
        GestureExaggerationOption,
        LipAnimSpeedOption,
        LipAnimDelayOption,
    ];
}

/// <summary>
/// Integration for the LipGenerator tool, to generate lip files.
/// </summary>
public sealed class LipGeneratorWrapper(IFileSystem fileSystem, ILogger logger, ConfigValues registrationConfigValues) : ILipGenerator {
    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("b1be1766-4a4e-4329-952e-60007fd08bc9"),
        "LipGenerator",
        "A tool for generating Skyrim lip sync files from WAV files and voice text.",
        "Lip Generator",
        typeof(ILipGenerator),
        new LipGeneratorWrapperRegistrationConfig(),
        new LipGeneratorWrapperRuntimeConfig(),
        []
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public IReadOnlyList<string> SupportedAudioExtensions { get; } = [".wav"];

    public async Task<IntegrationRunResult> GenerateLip(
        string wavPath,
        string text,
        ConfigValues config,
        CancellationToken token) {
        var toolPath = registrationConfigValues.Get(LipGeneratorWrapperRegistrationConfig.ToolPathOption);
        var language = config.TryGetStruct<Language>(LipGeneratorWrapperRuntimeConfig.LanguageOption);
        var gestureExaggeration = config.TryGetStruct(LipGeneratorWrapperRuntimeConfig.GestureExaggerationOption);
        var lipAnimSpeed = config.TryGetStruct(LipGeneratorWrapperRuntimeConfig.LipAnimSpeedOption);
        var lipAnimDelay = config.TryGetStruct(LipGeneratorWrapperRuntimeConfig.LipAnimDelayOption);

        if (string.IsNullOrEmpty(toolPath) || !fileSystem.File.Exists(toolPath)) {
            return new IntegrationRunResult(false, $"LipGenerator executable not found at: {toolPath}");
        }

        var tries = 0;
        while (true) {
            var result = await GenerateLipImplementation(toolPath, language, gestureExaggeration, lipAnimSpeed, lipAnimDelay, wavPath, text, token);
            if (result.Succeeded) return result;

            tries++;
            if (tries >= 3) return result;
        }
    }

    private async Task<IntegrationRunResult> GenerateLipImplementation(
        string toolPath,
        Language? language,
        float? gestureExaggeration,
        float? lipAnimSpeed,
        float? lipAnimDelay,
        string wavPath,
        string text,
        CancellationToken token) {
        // Remove quotes from text to avoid issues with command line arguments
        text = text.Replace("\"", string.Empty);

        var arguments = $"\"{wavPath}\" \"{text}\"";

        if (language.HasValue) arguments += $" -Language:{ConvertLanguage(language.Value)}";
        if (gestureExaggeration.HasValue) arguments += $" -GestureExaggeration:{gestureExaggeration.Value}";
        if (lipAnimDelay.HasValue) arguments += $" -LipAnimDelay:{lipAnimDelay.Value}";
        if (lipAnimSpeed.HasValue) arguments += $" -LipAnimSpeed:{lipAnimSpeed.Value}";

        try {
            var (exitCode, output, error) = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (exitCode != 0) {
                logger.Here().Warning("LipGenerator failed to generate lip file for {WavPath}\nOutput: {Output}\nError: {Error}",
                    wavPath,
                    output,
                    error);
                return new IntegrationRunResult(false, $"LipGenerator failed to generate lip file for {wavPath}: {error}");
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
            Language.French => "French",
            Language.German => "German",
            Language.Spanish => "Spanish",
            Language.Italian => "Italian",
            Language.Korean => "Korean",
            Language.Japanese => "Japanese",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        };
    }
}
