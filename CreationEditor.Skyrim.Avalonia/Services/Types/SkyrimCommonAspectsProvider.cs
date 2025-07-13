using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Types;

public sealed class SkyrimCommonAspectsProvider : IMutagenCommonAspectsProvider {
    public Type AddonNodeRecordType { get; } = typeof(IAddonNodeGetter);
    public Type SoundDescriptorRecordType { get; } = typeof(ISoundDescriptorGetter);

    public int? GetAddonNodeIndex(IMajorRecordGetter record) {
        if (record is IAddonNodeGetter addonNode) {
            return addonNode.NodeIndex;
        }

        return null;
    }
}
