using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Audio;
using CreationEditor.Services.Integrations.Lip;
using NAudio.Wave;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using Serilog;
namespace ReleasePipeline.Services.Actions;

public sealed class EncodeAudioConfig : IConfigDefinition {
    public static IntegrationConfigOption LipFuzerOption { get; } = new(
        "Lip Fuzer",
        typeof(ILipFuzer)
    );

    public static IntegrationConfigOption AudioEncoderOption { get; } = new(
        "Audio Encoder",
        typeof(IAudioEncoder)
    );

    public static ListConfigOption<AudioEncodingRule> EncodingRulesOption { get; } = new(
        "Encoding Rules",
        "The first rule whose file filter matches a file determines the bitrate used to encode it, overwriting the bitrate configured in the audio encoder"
    );

    public static BoolConfigOption DeleteSourceFilesOption { get; } = new(
        "Delete Source Files",
        "If enabled, the original audio files are deleted after they are successfully encoded",
        true
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        LipFuzerOption,
        AudioEncoderOption,
        EncodingRulesOption,
        DeleteSourceFilesOption,
    ];
}

/// <summary>
/// Pipeline action that encodes audio files in the working directory using a configured audio encoder integration.
/// Rule map a file path wildcard to a target bitrate.
/// Fuz files are extracted first to get the audio files, then the audio files are encoded,
/// and finally fuz files are generated again to include the newly encoded audio files.
/// </summary>
public sealed class EncodeAudioAction(
    IIntegrationRegistry registry,
    IFileSystem fileSystem,
    ILogger logger) : IPipelineAction {
    public Guid Id { get; } = new("c3f2a1b4-6d5e-4f8a-9b2c-1d4e5f6a7b8c");
    public string DisplayName => "Encode Audio";
    public string Description => "Encodes audio files in the release folder into a compressed format using a configured audio encoder integration.";
    public bool RequiresInPlaceModification => true;
    public IConfigDefinition ConfigDefinition { get; } = new EncodeAudioConfig();
    public ConfigValues ConfigValues { get; set; } = new();

    public async Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token) {
        try {
            var rules = ConfigValues.Get(EncodeAudioConfig.EncodingRulesOption);
            if (rules.Count == 0) {
                return PipelineActionResult.Fail("No encoding rules are configured. Add at least one rule.");
            }

            // Extract fuz files first to get the audio files
            var lipFuzerOption = ConfigValues.Get(EncodeAudioConfig.LipFuzerOption);
            var lipFuzer = registry.TryGetIntegration<ILipFuzer>(lipFuzerOption.IntegrationId);
            if (lipFuzer is null) {
                return PipelineActionResult.Fail("No lip fuzer integration is configured for this action.");
            }

            var extractResult = await lipFuzer.ExtractFuz(context.WorkingDirectory, lipFuzerOption.ConfigValues, token);
            if (!extractResult.Succeeded) {
                return PipelineActionResult.Fail($"Failed to extract fuz files: {extractResult.Message}");
            }

            // Now encode the audio files
            var audioEncoderOption = ConfigValues.Get(EncodeAudioConfig.AudioEncoderOption);
            var audioEncoder = registry.TryGetIntegration<IAudioEncoder>(audioEncoderOption.IntegrationId);
            if (audioEncoder is null) {
                return PipelineActionResult.Fail("No audio encoder integration is configured for this action.");
            }

            // Re-encode already encoded files if the rule allows it, otherwise skip them
            await Decode(context, token, rules, audioEncoder, audioEncoderOption);

            var deleteSourceFiles = ConfigValues.Get(EncodeAudioConfig.DeleteSourceFilesOption);

            // Encode unencoded files into the target audio format
            var (encodedCount, skippedCount, failedCount, encodeResult) =
                await EncodeFiles(context, token, deleteSourceFiles, rules, audioEncoder, audioEncoderOption);

            if (encodeResult is not null) {
                return encodeResult;
            }

            // Fuze lips files again to include the newly encoded audio files
            if (await FuzeLips(context, token, rules, lipFuzer, audioEncoder) is {} fuzeResult) {
                return fuzeResult;
            }

            var message = $"Encoded {encodedCount} audio file(s), skipped {skippedCount}, failed {failedCount}.";
            return failedCount > 0
                ? PipelineActionResult.Fail(message)
                : PipelineActionResult.Pass(message);
        } catch (Exception e) {
            Console.WriteLine(e);

            return PipelineActionResult.Fail("message");
        }
    }

    private async Task Decode(
        PipelineContext context,
        CancellationToken token,
        IReadOnlyList<ConfiguredValues<AudioEncodingRule>> rules,
        IAudioEncoder audioEncoder,
        IntegrationConfiguration audioEncoderOption) {
        foreach (var extension in audioEncoder.SupportedEncodedExtensions) {
            var files = fileSystem.Directory.GetFiles(context.WorkingDirectory, $"*{extension}", SearchOption.AllDirectories);
            await Parallel.ForEachAsync(
                files,
                token,
                async (file, cancellationToken) => {
                    cancellationToken.ThrowIfCancellationRequested();

                    foreach (var rule in rules) {
                        if (!rule.ConfigDefinition.Matches(rule.ConfigValues, file)) continue;

                        // If the rule is specified to be re-encoded or not encoded at all, decode the file back
                        // to an unencoded format for re-encoding or using the decoded file
                        if (!rule.ConfigValues.Get(AudioEncodingRule.ReEncodeOption)
                         && !rule.ConfigValues.Get(AudioEncodingRule.DontEncodeOption)) {
                            continue;
                        }

                        // Create a new ConfigValues instance for the audio encoder with the bitrate set in the rule
                        var customAudioEncoderOptions = new ConfigValues(audioEncoderOption.ConfigValues);
                        var bitrateConfigOption = rule.ConfigDefinition.GetConfigOption(nameof(AudioEncodingRule.BitrateOption));
                        var audioEncoderOptionBitrate = audioEncoder.GetRegistrationConfigOption(nameof(IAudioEncoderRuntimeConfigDefinition.BitrateOption));
                        if (bitrateConfigOption is not null && audioEncoderOptionBitrate is not null) {
                            var bitrate = rule.ConfigValues.Get(AudioEncodingRule.BitrateOption);
                            customAudioEncoderOptions.Set(audioEncoderOptionBitrate.Name, bitrate);
                        }

                        // Decode the encoded file back to an unencoded format
                        // Do re-encoding in common loop below, so we don't duplicate the logic for encoding and deleting source files
                        var decodedFilePath = fileSystem.Path.ChangeExtension(file, audioEncoder.SupportedUnencodedExtensions[0]);
                        var decodingResult = await audioEncoder.Decode(file, decodedFilePath, customAudioEncoderOptions, cancellationToken);
                        if (!decodingResult.Succeeded) {
                            logger.Here().Warning("Audio decoding failed for {File}: {Message}", file, decodingResult.Message);
                            continue;
                        }

                        // Delete encoded file if decoding was successful
                        fileSystem.File.Delete(file);
                    }
                });
        }
    }


    private async Task<(int encodedCount, int skippedCount, int failedCount, PipelineActionResult? execute)> EncodeFiles(
        PipelineContext context,
        CancellationToken token,
        bool deleteSourceFiles,
        IReadOnlyList<ConfiguredValues<AudioEncodingRule>> rules,
        IAudioEncoder audioEncoder,
        IntegrationConfiguration audioEncoderOption) {
        var encodedCount = 0;
        var skippedCount = 0;
        var failedCount = 0;

        foreach (var extension in audioEncoder.SupportedUnencodedExtensions) {
            var unencodedAudioFiles = fileSystem.Directory.GetFiles(context.WorkingDirectory, $"*{extension}", SearchOption.AllDirectories);
            await Parallel.ForEachAsync(unencodedAudioFiles,
                token,
                async (unencodedAudioFile, t) => {
                    t.ThrowIfCancellationRequested();

                    var relativePath = fileSystem.Path.GetRelativePath(context.WorkingDirectory, unencodedAudioFile);
                    var rule = rules.FirstOrDefault(r => r.ConfigDefinition.Matches(r.ConfigValues, relativePath)
                     || r.ConfigDefinition.Matches(r.ConfigValues, fileSystem.Path.GetFileName(relativePath)));
                    if (rule is null) {
                        skippedCount++;
                        return;
                    }

                    // Convert the audio file to mono before encoding
                    var forceMono = rule.ConfigValues.Get(AudioEncodingRule.ForceMonoOption);
                    if (forceMono) {
                        try {
                            var reader = new AudioFileReader(unencodedAudioFile);
                            if (reader.WaveFormat.Channels != 1) {
                                var tempMonoPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
                                tempMonoPath = fileSystem.Path.ChangeExtension(tempMonoPath, extension);
                                WaveFileWriter.CreateWaveFile16(tempMonoPath, reader.ToMono());

                                // Dispose to free the file lock before moving the file to the original path
                                await reader.DisposeAsync();
                                fileSystem.File.Move(tempMonoPath, unencodedAudioFile, true);
                            } else {
                                await reader.DisposeAsync();
                            }
                        } catch (Exception e) {
                            logger.Here().Error(e, "Failed to convert {File} to mono, skipping encoding", unencodedAudioFile);
                        }
                    }

                    // Don't encode the file if the rule specifies to skip encoding
                    var dontEncode = rule.ConfigValues.Get(AudioEncodingRule.DontEncodeOption);
                    if (dontEncode) return;

                    // Set the bitrate for the audio encoder based on the matching rule
                    var bitrate = rule.ConfigValues.Get(AudioEncodingRule.BitrateOption);
                    var audioEncoderOptionBitrate = audioEncoder.GetRuntimeConfigOption(nameof(IAudioEncoderRuntimeConfigDefinition.BitrateOption));
                    if (audioEncoderOptionBitrate is null) {
                        logger.Here().Warning("Audio encoder integration does not define a bitrate option, skipping encoding for {File}", unencodedAudioFile);
                        return;
                    }
                    audioEncoderOption.ConfigValues.Set(audioEncoderOptionBitrate.Name, bitrate);

                    // Encode the audio file into the target format using the configured audio encoder
                    var targetPath = fileSystem.Path.ChangeExtension(unencodedAudioFile, audioEncoder.SupportedEncodedExtensions[0]);
                    var result = await audioEncoder.Encode(unencodedAudioFile, targetPath, audioEncoderOption.ConfigValues, t);

                    if (!result.Succeeded) {
                        failedCount++;
                        logger.Here().Warning("Audio encoding failed for {File}: {Message}", unencodedAudioFile, result.Message);
                        return;
                    }

                    encodedCount++;

                    // Delete the original unencoded audio file if configured to do so
                    if (deleteSourceFiles) {
                        fileSystem.File.Delete(unencodedAudioFile);
                    }
                });
        }

        return (encodedCount, skippedCount, failedCount, null);
    }

    private async Task<PipelineActionResult?> FuzeLips(
        PipelineContext context,
        CancellationToken token,
        IReadOnlyList<ConfiguredValues<AudioEncodingRule>> rules,
        ILipFuzer lipFuzer,
        IAudioEncoder audioEncoder) {
        var deleteSourceFiles = ConfigValues.Get(EncodeAudioConfig.DeleteSourceFilesOption);
        var dataSource = new FileSystemDataSource(fileSystem, context.WorkingDirectory);

        var audioExtensions = audioEncoder.SupportedEncodedExtensions.Concat(audioEncoder.SupportedUnencodedExtensions).ToArray();
        foreach (var audioExtension in audioExtensions) {
            var audioFiles = fileSystem.Directory.GetFiles(context.WorkingDirectory, $"*{audioExtension}", SearchOption.AllDirectories);
            await Parallel.ForEachAsync(audioFiles, token, (audioFile, t) => Fuze(audioFile, audioExtension, t));
        }

        return null;

        async ValueTask Fuze(string audioFile, string audioExtension, CancellationToken t) {
            if (t.IsCancellationRequested) return;

            foreach (var rule in rules) {
                if (!rule.ConfigDefinition.Matches(rule.ConfigValues, audioFile)) continue;

                var packageFuz = rule.ConfigValues.Get(AudioEncodingRule.PackageFuzOption);
                if (!packageFuz) break;

                // Fuze lips files
                var relativePath = fileSystem.Path.GetRelativePath(context.WorkingDirectory, audioFile);
                var fileLink = new DataSourceFileLink(dataSource, relativePath);
                var fuzerConfigValues = new ConfigValues();
                var audioExtensionConfigOption = lipFuzer.GetRuntimeConfigOption(nameof(ILipFuzerRuntimeConfigDefinition.AudioExtensionOption));
                if (audioExtensionConfigOption is not null) {
                    fuzerConfigValues.Set(audioExtensionConfigOption.Name, audioExtension);
                }
                var result = await lipFuzer.PackageFuz(fileLink, fuzerConfigValues, CancellationToken.None);
                if (!result.Succeeded) continue;

                // Delete source files if configured to do so and if the fuz file exists
                if (!deleteSourceFiles) continue;

                var fuzFile = fileSystem.Path.ChangeExtension(audioFile, lipFuzer.FuzeFileExtension);
                if (!fileSystem.File.Exists(fuzFile)) continue;

                var lipFile = fileSystem.Path.ChangeExtension(audioFile, lipFuzer.LipFileExtension);
                if (fileSystem.File.Exists(lipFile)) {
                    fileSystem.File.Delete(lipFile);
                }

                foreach (var encodedExtension in audioExtensions) {
                    var encodedAudio = fileSystem.Path.ChangeExtension(audioFile, encodedExtension);
                    if (fileSystem.File.Exists(encodedAudio)) {
                        fileSystem.File.Delete(encodedAudio);
                    }
                }
            }
        }
    }
}
