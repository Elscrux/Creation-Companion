using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using HKX2;
using Mutagen.Bethesda.Assets;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class BehaviorAnimObjectLinkParser(IAssetTypeService assetTypeService, ILogger logger) : IFileParser<string> {
    public string Name => "Behavior Anim Objects";
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

        var animObjIndices = new HashSet<int>();
        foreach (var hkbBehaviorGraphStringData in objects.Values.OfType<hkbBehaviorGraphStringData>()) {
            var loadIndex = hkbBehaviorGraphStringData.m_eventNames.IndexOf("AnimObjLoad");
            if (loadIndex != -1) {
                animObjIndices.Add(loadIndex);
            }

            var drawIndex = hkbBehaviorGraphStringData.m_eventNames.IndexOf("AnimObjDraw");
            if (drawIndex != -1) {
                animObjIndices.Add(drawIndex);
            }
        }

        if (animObjIndices.Count == 0) yield break;

        foreach (var hkbClipTriggerArray in objects.Values.OfType<hkbClipTriggerArray>()) {
            foreach (var hkbClipTrigger in hkbClipTriggerArray.m_triggers) {
                if (animObjIndices.Contains(hkbClipTrigger.m_event.m_id)) {
                    if (hkbClipTrigger.m_event.m_payload is hkbStringEventPayload hkbStringEventPayload) {
                        yield return hkbStringEventPayload.m_data.Trim();
                    }
                }
            }
        }

        foreach (var hkbStateMachineEventPropertyArray in objects.Values.OfType<hkbStateMachineEventPropertyArray>()) {
            foreach (var hkbEventProperty in hkbStateMachineEventPropertyArray.m_events) {
                if (animObjIndices.Contains(hkbEventProperty.m_id)) {
                    if (hkbEventProperty.m_payload is hkbStringEventPayload hkbStringEventPayload) {
                        yield return hkbStringEventPayload.m_data.Trim();
                    }
                }
            }
        }
    }
}
