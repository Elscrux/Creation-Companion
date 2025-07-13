using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using nifly;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifSoundLinkParser(IAssetTypeService assetTypeService) : IFileParser<string> {
    public string Name => "Nif Sounds";
    public IEnumerable<string> FileExtensions => assetTypeService.Provider.Model.FileExtensions;

    public IEnumerable<string> ParseFile(string filePath, IFileSystem fileSystem) {
        if (assetTypeService.GetAssetType(filePath) != assetTypeService.Provider.Model) return [];

        var results = new HashSet<string>();

        if (!fileSystem.File.Exists(filePath)) return results;

        using var nif = new NifFile();
        nif.Load(filePath);

        if (!nif.IsValid()) return results;

        using var niHeader = nif.GetHeader();
        for (uint i = 0; i < niHeader.GetStringCount(); i++) {
            var str = niHeader.GetStringById(i);
            if (str is null) continue;

            var span = str.AsSpan();
            if (!span.StartsWith("sound", StringComparison.OrdinalIgnoreCase)) continue;

            const string soundPlay = "soundPlay.";
            const string soundPlayAt = "SoundPlayAt.";
            const string soundStop = "SoundStop.";
            const string sound = "Sound: ";

            if (span.StartsWith(soundPlay, StringComparison.OrdinalIgnoreCase)) {
                TryAddAssetFromString(str.AsSpan(soundPlay.Length).ToString());
            } else if (span.StartsWith(soundPlayAt, StringComparison.OrdinalIgnoreCase)) {
                TryAddAssetFromString(str.AsSpan(soundPlayAt.Length).ToString());
            } else if (span.StartsWith(soundStop, StringComparison.OrdinalIgnoreCase)) {
                TryAddAssetFromString(str.AsSpan(soundStop.Length).ToString());
            } else if (span.StartsWith(sound, StringComparison.OrdinalIgnoreCase)) {
                TryAddAssetFromString(str.AsSpan(sound.Length).ToString());
            }
        }

        return results;

        void TryAddAssetFromString(string soundEditorId) {
            if (string.IsNullOrEmpty(soundEditorId)) return;

            results.Add(soundEditorId);
        }
    }
}