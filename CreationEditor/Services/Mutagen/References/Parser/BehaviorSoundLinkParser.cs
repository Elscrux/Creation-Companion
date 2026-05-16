using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using HKX2;
using Mutagen.Bethesda.Assets;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class BehaviorSoundLinkParser(IAssetTypeService assetTypeService, ILogger logger) : IFileParser<string> {
    public string Name => "Behavior Sounds";
    public IAssetType AssetType => assetTypeService.Provider.Behavior;

    public IEnumerable<string> ParseFile(string actualFilePath, DataSourceFileLink fileLink) {
        using var rs = File.OpenRead(actualFilePath);
        var br = new BinaryReaderEx(rs);
        var des = new PackFileDeserializer();

        Dictionary<uint, IHavokObject> objects;
        try {
            des.Deserialize(br, out objects);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to parse Behavior file {FilePath}: {Exception}", actualFilePath, e);
            yield break;
        }

        foreach (var hkbBehaviorGraphStringData in objects.Values.OfType<hkbBehaviorGraphStringData>()) {
            foreach (var mEventName in hkbBehaviorGraphStringData.m_eventNames) {
                const string soundPlay = "SoundPlay.";
                const string soundStop = "SoundStop.";

                if (mEventName.StartsWith(soundPlay, StringComparison.OrdinalIgnoreCase)) {
                    yield return mEventName[soundPlay.Length..].Trim();
                } else if (mEventName.StartsWith(soundStop, StringComparison.OrdinalIgnoreCase)) {
                    yield return mEventName[soundStop.Length..].Trim();
                }
            }
        }
    }
}
