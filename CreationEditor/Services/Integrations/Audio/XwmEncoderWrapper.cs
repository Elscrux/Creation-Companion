using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using NAudio.Wave;
using Serilog;
namespace CreationEditor.Services.Integrations.Audio;

public sealed class XwmEncoderWrapperRegistrationConfig : IConfigDefinition {
    public static FileConfigOption ToolPathOption { get; } = new(
        "xWMAEncode Path",
        "Path to the xWMAEncode executable",
        autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Game, @"Tools\Audio\xwmaencode.exe")]);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ToolPathOption,
    ];
}

public enum XwmEncoderBitrate {
    _20000 = 20000,
    _32000 = 32000,
    _48000 = 48000,
    _64000 = 64000,
    _96000 = 96000,
    _160000 = 160000,
    _192000 = 192000,
}

public interface IAudioEncoderRuntimeConfigDefinition : IConfigDefinition {
    static abstract EnumConfigOption BitrateOption { get; }
}

public sealed class XwmEncoderWrapperRuntimeConfig : IConfigDefinition {
    public static EnumConfigOption BitrateOption { get; } = new(
        "Bitrate",
        typeof(XwmEncoderBitrate),
        "Higher bitrates provide better audio quality but larger file sizes",
        XwmEncoderBitrate._96000);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        BitrateOption,
    ];
}

/// <summary>
/// Integration for the xwmaencode tool, to encode WAV files into XWM audio files.
/// </summary>
public sealed class XwmEncoderWrapper(ILogger logger, IFileSystem fileSystem, ConfigValues registrationConfigValues) : IAudioEncoder {
    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("ab99834b-ee2c-4e02-bd03-383eae8810d6"),
        "xWMAEncode",
        "A tool for encoding WAV files into XWM audio files.",
        "Audio Encoder",
        typeof(IAudioEncoder),
        new XwmEncoderWrapperRegistrationConfig(),
        new XwmEncoderWrapperRuntimeConfig(),
        []
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public IReadOnlyList<string> SupportedEncodedExtensions { get; } = [".xwm"];
    public IReadOnlyList<string> SupportedUnencodedExtensions { get; } = [".wav"];

    public async Task<IntegrationRunResult> Encode(
        string sourcePath,
        string encodedPath,
        ConfigValues config,
        CancellationToken token) {
        var toolPath = registrationConfigValues.Get(XwmEncoderWrapperRegistrationConfig.ToolPathOption);
        if (string.IsNullOrEmpty(toolPath)) {
            return new IntegrationRunResult(false, $"xwmaencode executable not found at: {toolPath}");
        }

        var arguments = $"\"{sourcePath}\" \"{encodedPath}\"";

        var bitrateOption = config.Get<XwmEncoderBitrate>(XwmEncoderWrapperRuntimeConfig.BitrateOption);
        var bitrateValue = (int) bitrateOption;
        arguments += $" -b {bitrateValue}";

        try {
            token.ThrowIfCancellationRequested();
            var result = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (result.ExitCode != 0) {
                // In case the encoding fails, we can try to re-save the source file and try again
                var reader = new AudioFileReader(sourcePath);
                var tempPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
                tempPath = fileSystem.Path.ChangeExtension(tempPath, SupportedUnencodedExtensions[0]);
                WaveFileWriter.CreateWaveFile16(tempPath, reader.ToMono());
                await reader.DisposeAsync();
                fileSystem.File.Move(tempPath, sourcePath, true);

                result = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

                if (result.ExitCode != 0) {
                    logger.Here().Warning("XwmEncoder exited with code {ExitCode} for file {SourcePath}:\nOutput: {Output}\nError: {Error}",
                        result.ExitCode,
                        sourcePath,
                        result.Output,
                        result.Error);
                    return new IntegrationRunResult(false, $"xwmaencode failed with exit code {result.ExitCode} for {sourcePath}: {result.Error}");
                }
            }

            logger.Here().Debug("Encoded xwm from {SourcePath} to {TargetPath} with bitrate {Bitrate}", sourcePath, encodedPath, bitrateValue);
            return new IntegrationRunResult(true, $"Encoded {sourcePath} to {encodedPath}.");
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Audio encoding was cancelled: {SourcePath}", sourcePath);
            return new IntegrationRunResult(false, $"Audio encoding of {sourcePath} was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to encode audio: {SourcePath}", sourcePath);
            return new IntegrationRunResult(false, $"Failed to encode {sourcePath}: {e.Message}");
        }
    }

    public async Task<IntegrationRunResult> Decode(string encodedPath, string targetPath, ConfigValues config, CancellationToken token) {
        var toolPath = registrationConfigValues.Get(XwmEncoderWrapperRegistrationConfig.ToolPathOption);
        if (string.IsNullOrEmpty(toolPath)) {
            return new IntegrationRunResult(false, $"xwmaencode executable not found at: {toolPath}");
        }

        var arguments = $"\"{encodedPath}\" \"{targetPath}\"";

        try {
            token.ThrowIfCancellationRequested();
            var result = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (result.ExitCode != 0) {
                logger.Here().Warning("XwmEncoder exited with code {ExitCode} for file {SourcePath}:\nOutput: {Output}\nError: {Error}",
                    result.ExitCode,
                    encodedPath,
                    result.Output,
                    result.Error);
                return new IntegrationRunResult(false, $"xwmaencode failed with exit code {result.ExitCode} for {encodedPath}: {result.Error}");
            }

            logger.Here().Debug("Decoded xwm from {SourcePath} to {TargetPath}", encodedPath, targetPath);
            return new IntegrationRunResult(true, $"Decoded {encodedPath} to {targetPath}.");
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Audio decoding was cancelled: {SourcePath}", encodedPath);
            return new IntegrationRunResult(false, $"Audio encoding of {encodedPath} was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to encode audio: {SourcePath}", encodedPath);
            return new IntegrationRunResult(false, $"Failed to encode {encodedPath}: {e.Message}");
        }
    }
}
