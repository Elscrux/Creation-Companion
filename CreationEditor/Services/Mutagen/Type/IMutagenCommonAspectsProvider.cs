using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Type;

public interface IMutagenCommonAspectsProvider {
    System.Type AddonNodeRecordType { get; }
    System.Type SoundDescriptorRecordType { get; }

    int? GetAddonNodeIndex(IMajorRecordGetter record);
}
