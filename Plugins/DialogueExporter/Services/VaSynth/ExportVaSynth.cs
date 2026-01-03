using System.Globalization;
using System.Text.RegularExpressions;
using CreationEditor;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CsvHelper;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Noggog;
using Serilog;
namespace DialogueExporter.Services.VaSynth;

public sealed class ExportVaSynth(
    ILogger logger,
    IDataSourceService dataSourceService,
    IEditorEnvironment environment) {
    public void Export(IModGetter mod, string voiceTypeMappingCsvFile, string outputDirectory, string questEditorRegex = ".*", string vocoder = "hifi") {
        logger.Here().Verbose("Start exporting VA Synth of quests matching RegEx {Quest} in mod {Mod} with voice type mapping {VoiceTypeMapping} to {OutputDirectory}",
            questEditorRegex,
            mod.ModKey,
            voiceTypeMappingCsvFile,
            outputDirectory);

        var linkCache = environment.LinkCache;

        using var streamReader = new StreamReader(voiceTypeMappingCsvFile);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        var voiceTypeMappings = csvReader
            .GetRecords<VoiceTypeMappingRecord>()
            .ToDictionary(x => x.VoiceType, x => x.Model);

        using var writer = new StreamWriter(outputDirectory);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteHeader<VaSynthRecord>();

        var assetLinkCache = linkCache.CreateImmutableAssetLinkCache();
        var voiceTypeAssetLookup = assetLinkCache.GetComponent<VoiceTypeAssetLookup>();

        foreach (var questGroup in mod.EnumerateMajorRecords<IDialogTopicGetter>().GroupBy(t => t.Quest.FormKey)) {
            if (!linkCache.TryResolve<IQuestGetter>(questGroup.Key, out var quest)) {
                logger.Here().Error("Quest {Quest} not found in mod, skipping topics {Topics}", questGroup.Key, string.Join(", ", questGroup.Select(t => t.FormKey)));
                continue;
            }

            if (quest.EditorID is null) continue;

            var match = Regex.Match(quest.EditorID, questEditorRegex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (!match.Success) {
                logger.Here().Verbose("Skipping quest {EditorID} as it does not match the regex {Regex}", quest.EditorID, questEditorRegex);
                continue;
            }

            foreach (var topic in questGroup) {
                foreach (var responses in topic.Responses) {
                    var dataRelativePaths = voiceTypeAssetLookup.GetVoiceLineFilePaths(responses).ToArray();
                    if (dataRelativePaths.Length == 0) continue;

                    foreach (var dataRelativePath in dataRelativePaths) {
                        var path = dataRelativePath.Path;
                        if (dataSourceService.FileExists(path)) continue;

                        var lastSeparator = path.LastIndexOf(Path.DirectorySeparatorChar);
                        var voiceTypeFolder = path[..lastSeparator];
                        lastSeparator = voiceTypeFolder.LastIndexOf(Path.DirectorySeparatorChar);
                        var voiceType = voiceTypeFolder[(lastSeparator + 1)..];
                        var fileName = path[(lastSeparator + 1)..^4];

                        // WICastMagi_WICastMagicNonH_000073C7_1
                        var lastUnderscore = fileName.LastIndexOf('_');
                        var responseNumber = int.Parse(fileName[(lastUnderscore + 1)..]);
                        var response = responses.Responses.First(r => r.ResponseNumber == responseNumber);

                        if (response.Text.String is null) {
                            logger.Here().Warning("Response text is null for topic {FormKey} response number {ResponseNumber}, skipping", topic.FormKey, responseNumber);
                            continue;
                        }

                        if (response.Text.String.Length == 0) {
                            logger.Here().Warning("Response text is empty for topic {FormKey} response number {ResponseNumber}, skipping", topic.FormKey, responseNumber);
                            continue;
                        }

                        if (!voiceTypeMappings.TryGetValue(voiceType, out var voiceId) || voiceId.IsNullOrWhitespace()) {
                            logger.Here().Warning("Voice type {VoiceType} not found in mapping file, skipping topic {FormKey}", voiceType, topic.FormKey);
                            continue;
                        }

                        try {
                            csv.NextRecord();
                            csv.WriteRecord(new VaSynthRecord {
                                GameId = "skyrim",
                                VoiceId = voiceId,
                                Text = response.Text.String,
                                VcStyle = response.Emotion + ": " + response.ScriptNotes,
                                Vocoder = vocoder,
                                OutPath = '.' + path.Replace('\\', '/').TrimStart("Sound/Voice", StringComparison.OrdinalIgnoreCase).Replace(".fuz", ".wav"),
                            });
                        } catch (Exception e) {
                            logger.Here().Error(e, "Failed to write VA Synth record for topic {Topic} response number {ResponseNumber}", topic.FormKey, responseNumber);
                        }
                    }
                }
            }
        }

        logger.Here().Verbose("Finished exporting VA Synth of quests matching RegEx {Quest} in mod {Mod} with voice type mapping {VoiceTypeMapping} to {OutputDirectory}",
            questEditorRegex,
            mod.ModKey,
            voiceTypeMappingCsvFile,
            outputDirectory);
    }
}
