using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Config;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Audio;
using CreationEditor.Services.Integrations.Lip;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using NAudio.Wave;
using Serilog;
namespace LipGenerator.Services;

public sealed class LipFileGenerator(
    ILogger logger,
    IFileSystem fileSystem,
    IAssetController assetController,
    IEditorEnvironment editorEnvironment,
    IIntegrationRegistry integrationRegistry) {

    public async Task Run(
        IDataSource dataSource,
        bool cleanupFiles,
        IntegrationConfiguration lipGeneratorConfig,
        IntegrationConfiguration fuzGeneratorConfig,
        IntegrationConfiguration? audioEncoderConfig = null,
        int parallelizationDegree = -1,
        Action<int, string>? onProgress = null) {

        var lipGenerator = integrationRegistry.TryGetIntegration<ILipGenerator>(lipGeneratorConfig.IntegrationId);
        if (lipGenerator is null) {
            throw new InvalidOperationException($"No lip generator integration registered for id {lipGeneratorConfig.IntegrationId}");
        }

        var fuzGenerator = integrationRegistry.TryGetIntegration<ILipFuzer>(fuzGeneratorConfig.IntegrationId);
        if (fuzGenerator is null) {
            throw new InvalidOperationException($"No lip fuzer integration registered for id {fuzGeneratorConfig.IntegrationId}");
        }

        var audioEncoder = audioEncoderConfig is null
            ? null
            : integrationRegistry.TryGetIntegration<IAudioEncoder>(audioEncoderConfig.IntegrationId);

        var audioExtensionConfigOption = fuzGenerator.GetRuntimeConfigOption(nameof(ILipFuzerRuntimeConfigDefinition.AudioExtensionOption));
        if (audioExtensionConfigOption is null) {
            throw new InvalidOperationException($"Lip fuzer integration {fuzGenerator.Descriptor.DisplayName} does not have an audio extension config option");
        }

        logger.Here().Information(
            "Starting lip generation with tools: LipGenerator={LipGen}, FuzGenerator={FuzGen}, Encoder={Encoder}, ParallelizationDegree={ParallelizationDegree}, DeleteIntermediateFiles={DeleteIntermediateFiles}",
            lipGenerator.GetType().Name,
            fuzGenerator.GetType().Name,
            audioEncoder?.GetType().Name ?? "None",
            parallelizationDegree,
            cleanupFiles);

        var voiceDirectory = new DataSourceDirectoryLink(dataSource, dataSource.FileSystem.Path.Combine(SkyrimSoundAssetType.Instance.BaseFolder, "Voice"));

        onProgress?.Invoke(0, "Scanning for mod folders...");

        var modVoiceDirectories = voiceDirectory.EnumerateDirectoryLinks(false).ToList();
        var degree = Math.Clamp(parallelizationDegree, 1, Environment.ProcessorCount);
        var totalMods = modVoiceDirectories.Count;
        var processedFiles = 0;
        var currentMod = 0;

        foreach (var modVoiceDirectory in modVoiceDirectories) {
            currentMod++;
            var modFileName = modVoiceDirectory.Name;
            onProgress?.Invoke((int) (currentMod / (float) totalMods * 100), $"Processing {modFileName} ({currentMod}/{totalMods})...");

            // Decode audio files to prepare them for lip generation
            if (audioEncoder is not null) {
                foreach (var encodedExtension in audioEncoder.SupportedEncodedExtensions) {
                    foreach (var encodedFile in modVoiceDirectory.EnumerateFileLinks("*" + encodedExtension, true)) {
                        // Create a new ConfigValues instance for the audio encoder with the bitrate set in the rule
                        var decodedFile = encodedFile.WithExtension(audioEncoder.SupportedUnencodedExtensions[0]);
                        await audioEncoder.Decode(encodedFile.FullPath, decodedFile.FullPath, audioEncoderConfig!.ConfigValues, CancellationToken.None);
                    }
                }
            }

            // Get audio files that need lip files generated
            var voiceFiles = lipGenerator.SupportedAudioExtensions
                .SelectMany(extension => modVoiceDirectory.EnumerateFileLinks("*" + extension, true))
                .Where(fileLink => {
                    var lipFileLink = fileLink.WithExtension(fuzGenerator.LipFileExtension);
                    return !lipFileLink.Exists();
                }).ToList();

            var workItems = new List<(string VoiceFullPath, string Text)>();
            foreach (var voiceFile in voiceFiles) {
                var text = GetText(voiceFile, modFileName);
                if (text is null) continue;

                workItems.Add((voiceFile.FullPath, text));
            }

            // Generate lip files
            var processedItems = 0;
            onProgress?.Invoke((int) (currentMod / (float) totalMods * 100), $"Generating {workItems.Count} lip files for {modFileName}...");
            logger.Here().Debug("Generating lip files for {ModFileName} with parallelization degree {Degree}", modFileName, degree);

            var mod = currentMod;
            await Parallel.ForEachAsync(
                workItems,
                new ParallelOptions { MaxDegreeOfParallelism = degree },
                async (workItem, _) => await GenerateLipFile(workItem.VoiceFullPath, workItem.Text));

            // Fuze lips files
            onProgress?.Invoke((int) (currentMod / (float) totalMods * 100), $"Generating .fuz files for {modFileName}...");
            logger.Here().Information("Generating .fuz files for {ModFileName}", modFileName);

            // Fallback to audio extension used in the lip generator if the audio encoder is not configured, to ensure that the fuzer has a valid audio extension to use
            if (audioEncoder is null) {
                var audioExtensionOption = fuzGenerator.GetRuntimeConfigOption(nameof(ILipFuzerRuntimeConfigDefinition.AudioExtensionOption));
                if (audioExtensionOption is not null) {
                    fuzGeneratorConfig.ConfigValues.Set(audioExtensionOption.Name, lipGenerator.SupportedAudioExtensions[0]);
                }
            }

            var fuzResult = await fuzGenerator.PackageFuz(modVoiceDirectory.FullPath, fuzGeneratorConfig.ConfigValues, CancellationToken.None);
            if (!fuzResult.Succeeded) {
                logger.Here().Warning("Lip fuzer failed for {ModFileName}: {Message}", modFileName, fuzResult.Message);
            }

            processedFiles += workItems.Count;

            // Cleanup intermediate files if requested
            if (cleanupFiles) {
                CleanupFiles();
            }
            continue;

            async Task GenerateLipFile(string voiceFullPath, string text) {
                var reader = new AudioFileReader(voiceFullPath);

                try {
                    // Check if already mono and 16-bit
                    var waveFormat = reader.WaveFormat;
                    if (waveFormat is not { Channels: 1, BitsPerSample: 16 }) {
                        // Otherwise, convert to mono 16-bit and save to a temporary file
                        var tempPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
                        var extension = fileSystem.Path.GetExtension(voiceFullPath);
                        tempPath = fileSystem.Path.ChangeExtension(tempPath, extension);

                        WaveFileWriter.CreateWaveFile16(tempPath, reader.ToMono());
                        await reader.DisposeAsync();

                        fileSystem.File.Move(tempPath, voiceFullPath, true);

                        logger.Here().Debug("Converted {WavPath} to mono 16-bit at {TempPath}", voiceFullPath, tempPath);
                    }

                    // Generate the lip using the configured lip generator integration
                    var lipResult = lipGenerator.GenerateLip(voiceFullPath, text, lipGeneratorConfig.ConfigValues, CancellationToken.None).GetAwaiter().GetResult();
                    if (!lipResult.Succeeded) {
                        logger.Here().Warning("Lip generation failed for {WavPath}: {Message}", voiceFullPath, lipResult.Message);
                        return;
                    }

                    // Encode audio using the configured encoder integration (skip if none selected)
                    if (audioEncoder is not null && audioEncoderConfig is not null) {
                        // fuzGeneratorConfig.ConfigValues.TryGet(nameof(IFuzGeneratorRuntimeConfigDefinition.AudioExtensionOption)) as string,
                        var encodedPath = dataSource.FileSystem.Path.ChangeExtension(voiceFullPath, audioEncoder.SupportedEncodedExtensions[0]);
                        var encodeResult = await audioEncoder.Encode(voiceFullPath, encodedPath, audioEncoderConfig.ConfigValues, CancellationToken.None);
                        if (!encodeResult.Succeeded) {
                            logger.Here().Warning("Audio encoding failed for {WavPath}: {Message}", voiceFullPath, encodeResult.Message);
                        }
                    }

                    var itemsProcessed = Interlocked.Increment(ref processedItems);
                    if (itemsProcessed % 10 == 0 || itemsProcessed == workItems.Count) {
                        onProgress?.Invoke((int) (mod / (float) totalMods * 100), $"{modFileName}: {itemsProcessed}/{workItems.Count}");
                    }
                } catch (Exception ex) {
                    logger.Here().Error(ex, "Error generating .lip files for {WavPath}", voiceFullPath);
                } finally {
                    await reader.DisposeAsync();
                }
            }

            void CleanupFiles() {
                // Enumerate all fuz files and delete the corresponding wav, lip, and other audio files if they exist
                Parallel.ForEach(
                    modVoiceDirectory.EnumerateFileLinks("*" + fuzGenerator.FuzeFileExtension, true),
                    new ParallelOptions { MaxDegreeOfParallelism = degree },
                    fileLink => {
                        try {
                            if (audioEncoder is not null) {
                                var extensions = audioEncoder.SupportedUnencodedExtensions.Concat(audioEncoder.SupportedEncodedExtensions);
                                foreach (var extension in extensions) {
                                    var otherAudioFileLink = fileLink.WithExtension(extension);
                                    if (otherAudioFileLink.Exists()) {
                                        assetController.Delete(otherAudioFileLink);
                                    }
                                }
                            }

                            foreach (var extension in lipGenerator.SupportedAudioExtensions) {
                                var otherAudioFileLink = fileLink.WithExtension(extension);
                                if (otherAudioFileLink.Exists()) {
                                    assetController.Delete(otherAudioFileLink);
                                }
                            }

                            var lipFileLink = fileLink.WithExtension(fuzGenerator.LipFileExtension);
                            if (lipFileLink.Exists()) {
                                assetController.Delete(lipFileLink);
                            }
                        } catch (Exception e) {
                            logger.Here().Warning(e, "Error cleaning up files for {FileLink}", fileLink.FullPath);
                        }
                    });

            }
        }

        onProgress?.Invoke(100, "Complete!");
        logger.Here().Information("Lip generation completed successfully. Processed {ProcessedFiles} files from {TotalMods} mods", processedFiles, totalMods);
    }

    private string? GetText(DataSourceFileLink voiceFile, string modFileName) {
        var fileName = voiceFile.NameWithoutExtension;
        var responseId = fileName[^1..];
        if (!int.TryParse(responseId, out var id)) {
            logger.Here().Warning("Skipping {VoiceFile} for {VoiceType} because it does not have a valid response ID like '_1', or '_2'",
                voiceFile.Name,
                voiceFile.ParentDirectory?.Name);
            return null;
        }

        var formId = fileName[^8..^2];
        if (!FormKey.TryFactory(formId + ":" + modFileName, out var formKey)) {
            logger.Here().Warning("Skipping {VoiceFile} for {VoiceType} because it does not have a valid form id like '00123ABC'",
                voiceFile.Name,
                voiceFile.ParentDirectory?.Name);
            return null;
        }

        if (!editorEnvironment.LinkCache.TryResolve<IDialogResponsesGetter>(formKey, out var responses)) {
            logger.Here().Warning("Skipping {VoiceFile} for {VoiceType} because the dialog {FormKey} does not exist in the load order {LoadOrder}",
                voiceFile.Name,
                voiceFile.ParentDirectory?.Name,
                formKey,
                string.Join(", ", editorEnvironment.LinkCache.ListedOrder.Select(m => m.ModKey.FileName)));
            return null;
        }

        var response = responses.Responses.FirstOrDefault(r => r.ResponseNumber == id);
        return response?.Text.String;
    }
}
