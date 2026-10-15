using System.Reflection;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using NiflySharp;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifSoundLinkParser(
    IAssetTypeService assetTypeService,
    ILogger logger) : IFileParser<string> {
    public string Name => "Nif Sounds";
    public IAssetType AssetType => assetTypeService.Provider.Model;

    public IEnumerable<string> ParseFile(Stream? stream, DataSourceFileLink fileLink) {
        var results = new HashSet<string>();
        if (stream is null) return results;

        try {
            var nif = new NifFile(stream);

            if (typeof(NiHeader).GetField("strings", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(nif.Header) is not List<NiString> strings) {
                return results;
            }

            foreach (var stringRef in strings) {
                var str = stringRef.Content;
                var span = str.AsSpan();
                if (!span.StartsWith("sound", DataRelativePath.PathComparison)) continue;

                const string soundPlay = "soundPlay.";
                const string soundPlayAt = "SoundPlayAt.";
                const string soundStop = "SoundStop.";
                const string sound = "Sound: ";

                if (span.StartsWith(soundPlay, DataRelativePath.PathComparison)) {
                    TryAddAssetFromString(str.AsSpan(soundPlay.Length).ToString());
                } else if (span.StartsWith(soundPlayAt, DataRelativePath.PathComparison)) {
                    TryAddAssetFromString(str.AsSpan(soundPlayAt.Length).ToString());
                } else if (span.StartsWith(soundStop, DataRelativePath.PathComparison)) {
                    TryAddAssetFromString(str.AsSpan(soundStop.Length).ToString());
                } else if (span.StartsWith(sound, DataRelativePath.PathComparison)) {
                    TryAddAssetFromString(str.AsSpan(sound.Length).ToString());
                }
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Error occurred while parsing nif file: {FilePath}", fileLink.FullPath);
        }

        return results;

        void TryAddAssetFromString(string soundEditorId) {
            if (string.IsNullOrEmpty(soundEditorId)) return;

            results.Add(soundEditorId.Trim());
        }
    }
}
