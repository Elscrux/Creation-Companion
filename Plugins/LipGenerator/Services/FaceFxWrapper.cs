using System.Diagnostics;
using System.IO.Abstractions;
using CreationEditor;
using Mutagen.Bethesda.Strings;
using NAudio.Wave;
using Serilog;
namespace LipGenerator.Services;

public sealed record FaceFxWrapperArgs(
    string FaceFxPath,
    string FonixDataPath,
    Language Language = Language.English) : ILipGeneratorArgs;

/// <summary>
/// Calls https://github.com/Nukem9/FaceFXWrapper
/// </summary>
public sealed class FaceFxWrapper(
    ILogger logger,
    IFileSystem fileSystem,
    FaceFxWrapperArgs args) : ILipGenerator {
    public string FaceFxWrapperPath { get; } = args.FaceFxPath;
    public Language Language { get; } = args.Language;
    public string FonixDataPath { get; } = args.FonixDataPath;

    public static IReadOnlyList<Language> SupportedLanguages { get; } = [Language.English];

    public bool GenerateLip(string wavPath, string text) {
        var lipPath = fileSystem.Path.ChangeExtension(wavPath, ".lip");

        using var waveFileReader = new WaveFileReader(wavPath);
        var totalTime = waveFileReader.TotalTime;
        if (totalTime.TotalSeconds < 0.5f) {
            logger.Here().Debug("Skipping lip generation for {WavPath} because duration is less than 0.5 seconds", wavPath);
            return false;
        }

        // Remove quotes from text to avoid issues with command line arguments
        text = text.Replace("\"", string.Empty);

        var resampledPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
        var process = new Process {
            StartInfo = {
                FileName = FaceFxWrapperPath,
                Arguments =
                    $"\"Skyrim\" \"USEnglish\" \"{FonixDataPath}\" \"{wavPath}\" \"{resampledPath}\" \"{lipPath}\" \"{text}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0) {
            logger.Here().Warning("FaceFxWrapper failed to generate lip file for {WavPath}:\nOutput: {Output}\nError: {Error}", wavPath, output, error);
            return false;
        }

        logger.Here().Debug("Generated lip for {WavPath} with text: {Text}", wavPath, text);
        return true;
    }
}
