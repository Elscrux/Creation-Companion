using System.Diagnostics;
using System.IO.Abstractions;
using CreationEditor;
using Mutagen.Bethesda.Strings;
using Serilog;
namespace LipGenerator.Services;

public sealed record LipGeneratorArgs(
    string LipgenPath,
    Language Language = Language.English,
    float GestureExaggeration = 1f,
    float LipAnimSpeed = 1f,
    float LipAnimDelay = 0f) : ILipGeneratorArgs;

public sealed class LipGeneratorWrapper(
    IFileSystem fileSystem,
    ILogger logger,
    LipGeneratorArgs args) : ILipGenerator {

    public string LipGeneratorPath { get; } = ValidatePath(fileSystem, args.LipgenPath);
    public Language Language { get; } = args.Language;
    public float GestureExaggeration { get; } = args.GestureExaggeration;
    public float LipAnimDelay { get; } = args.LipAnimDelay;
    public float LipAnimSpeed { get; } = args.LipAnimSpeed;

    public static IReadOnlyList<Language> SupportedLanguages { get; } = [
        Language.English,
        Language.French,
        Language.German,
        Language.Spanish,
        Language.Italian,
        Language.Korean,
        Language.Japanese
    ];

    private static string ValidatePath(IFileSystem fileSystem, string path) {
        if (!fileSystem.File.Exists(path)) throw new FileNotFoundException($"LipGenerator.exe not found at {path}");

        return path;
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
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }

    public bool GenerateLip(string wavPath, string text) {
        var tries = 0;

        while (true) {
            if (GenerateLipImplementation(wavPath, text, out var output, out var error)) return true;

            tries++;
            if (tries < 3) continue;

            logger.Here().Warning("LipGenerator failed to generate lip file for {WavPath}\nOutput: {Output}\nError: {Error}",
                wavPath,
                output,
                error);
            return false;
        }
    }

    private bool GenerateLipImplementation(string wavPath, string text, out string output, out string error) {
        text = text.Replace("\"", string.Empty);
        var process = new Process {
            StartInfo = {
                FileName = LipGeneratorPath,
                Arguments = $"\"{wavPath}\" \"{text}\""
                  + $" -Language:{ConvertLanguage(Language)}"
                  + $" -GestureExaggeration:{GestureExaggeration}"
                  + $" -LipAnimDelay:{LipAnimDelay}"
                  + $" -LipAnimSpeed:{LipAnimSpeed}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0) {
            return false;
        }

        logger.Here().Debug("Generated lip for {WavPath} with text: {Text}", wavPath, text);
        return true;
    }
}
