using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using NAudio.Wave;
using Serilog;
namespace LipGenerator.Services;

public sealed class LipFileGenerator(
    ILogger logger,
    IFileSystem fileSystem,
    IEditorEnvironment editorEnvironment,
    ILipGenerator lipGenerator,
    IFuzGenerator fuzGenerator,
    IAudioEncoder? audioEncoder = null) {

    public void Run(
        IDataSource dataSource,
        bool cleanupFiles,
        int parallelizationDegree = -1,
        Action<int, string>? onProgress = null) {

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
        var currentMod = 0;

        foreach (var modVoiceDirectory in modVoiceDirectories) {
            currentMod++;
            var modFileName = modVoiceDirectory.Name;
            onProgress?.Invoke((int) ((currentMod / (float) totalMods) * 100), $"Processing {modFileName} ({currentMod}/{totalMods})...");

            // Get audio files that need lip files generated
            var voiceFiles = modVoiceDirectory.EnumerateFileLinks("*.wav", true)
                .Concat(modVoiceDirectory.EnumerateFileLinks("*.xwm", true))
                .Where(fileLink => {
                    var lipFileLink = fileLink.WithExtension(".lip");
                    return !lipFileLink.Exists();
                }).ToList();

            var workItems = new List<(string VoiceFullPath, string Text)>();
            foreach (var voiceFile in voiceFiles) {
                var fileName = voiceFile.NameWithoutExtension;
                var responseId = fileName[^1..];
                if (!int.TryParse(responseId, out var id)) continue;

                var formId = fileName[^8..^2];
                if (!FormKey.TryFactory(formId + ":" + modFileName, out var formKey)) continue;

                if (editorEnvironment.LinkCache.TryResolve<IDialogResponsesGetter>(formKey, out var responses)) {
                    var response = responses.Responses.FirstOrDefault(r => r.ResponseNumber == id);
                    if (response?.Text.String is not {} text) continue;
                    if (text.Trim().Length == 0) continue;

                    workItems.Add((voiceFile.FullPath, text));
                }
            }

            // Generate lip files
            var processedItems = 0;
            onProgress?.Invoke((int) ((currentMod / (float) totalMods) * 100), $"Generating {workItems.Count} lip files for {modFileName}...");
            logger.Here().Debug("Generating lip files for {ModFileName} with parallelization degree {Degree}", modFileName, degree);

            var mod = currentMod;
            Parallel.ForEach(
                workItems,
                new ParallelOptions { MaxDegreeOfParallelism = degree },
                workItem => GenerateLipFile(workItem.VoiceFullPath, workItem.Text));

            // Generate fuz files
            onProgress?.Invoke((int) ((currentMod / (float) totalMods) * 100), $"Generating .fuz files for {modFileName}...");
            logger.Here().Information("Generating .fuz files for {ModFileName}", modFileName);

            var audioExt = audioEncoder is not null ? audioEncoder.AudioExtension : "wav";

            fuzGenerator.GenerateFuz(
                modVoiceDirectory.FullPath,
                modVoiceDirectory.FullPath,
                audioExt: audioExt);

            // Cleanup intermediate files if requested
            if (cleanupFiles) {
                CleanupFiles();
            }
            continue;

            void GenerateLipFile(string voiceFullPath, string text) {
                var reader = new AudioFileReader(voiceFullPath);

                try {
                    // Check if already mono and 16-bit
                    var waveFormat = reader.WaveFormat;
                    if (waveFormat is not { Channels: 1, BitsPerSample: 16 }) {
                        // Otherwise, convert to mono 16-bit and save to a temporary file
                        var tempPath = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetRandomFileName());
                        tempPath = fileSystem.Path.ChangeExtension(tempPath, ".wav");

                        WaveFileWriter.CreateWaveFile16(tempPath, reader.ToMono());
                        reader.Dispose();

                        fileSystem.File.Move(tempPath, voiceFullPath, overwrite: true);

                        logger.Here().Debug("Converted {WavPath} to mono 16-bit at {TempPath}", voiceFullPath, tempPath);
                    }

                    // Generate the lip using configured generator
                    if (!lipGenerator.GenerateLip(voiceFullPath, text)) {
                        logger.Here().Warning("Lip generation failed for {WavPath}", voiceFullPath);
                        return;
                    }

                    // Encode audio using configured encoder (skip if NoOpXwmEncoder)
                    if (audioEncoder is not null) {
                        var xwmPath = dataSource.FileSystem.Path.ChangeExtension(voiceFullPath, ".xwm");
                        audioEncoder.Encode(voiceFullPath, xwmPath);
                    }

                    var itemsProcessed = Interlocked.Increment(ref processedItems);
                    if (itemsProcessed % 10 == 0 || itemsProcessed == workItems.Count) {
                        onProgress?.Invoke((int) ((mod / (float) totalMods) * 100), $"{modFileName}: {itemsProcessed}/{workItems.Count}");
                    }
                } catch (Exception ex) {
                    logger.Here().Error(ex, "Error generating .lip files for {WavPath}", voiceFullPath);
                } finally {
                    reader.Dispose();
                }
            }

            void CleanupFiles() {
                // Enumerate all fuz files and delete the corresponding wav, lip, and other audio files if they exist
                Parallel.ForEach(
                    modVoiceDirectory.EnumerateFileLinks("*.fuz", true),
                    new ParallelOptions { MaxDegreeOfParallelism = degree },
                    fileLink => {
                        try {
                            if (audioEncoder is not null) {
                                var otherAudioFileLink = fileLink.WithExtension("." + audioEncoder.AudioExtension);
                                if (otherAudioFileLink.Exists()) {
                                    dataSource.FileSystem.File.Delete(otherAudioFileLink.FullPath);
                                }
                            }

                            var lipFileLink = fileLink.WithExtension(".lip");
                            if (lipFileLink.Exists()) {
                                dataSource.FileSystem.File.Delete(lipFileLink.FullPath);
                            }

                            var wavFileLink = fileLink.WithExtension(".wav");
                            if (wavFileLink.Exists()) {
                                dataSource.FileSystem.File.Delete(wavFileLink.FullPath);
                            }
                        } catch (Exception e) {
                            logger.Here().Warning(e, "Error cleaning up files for {FileLink}", fileLink.FullPath);
                        }
                    });
            }
        }

        onProgress?.Invoke(100, "Complete!");
        logger.Here().Information("Lip generation completed successfully. Processed {TotalMods} mods", totalMods);
    }
}
