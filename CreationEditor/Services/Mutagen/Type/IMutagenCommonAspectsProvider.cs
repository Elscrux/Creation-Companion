using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
namespace CreationEditor.Services.Mutagen.Type;

public interface IMutagenCommonAspectsProvider {
    System.Type AddonNodeRecordType { get; }
    System.Type SoundDescriptorRecordType { get; }
    System.Type BookType { get; }
    System.Type MessageType { get; }
    System.Type DialogTopic { get; }
    System.Type DialogResponses { get; }

    int? GetAddonNodeIndex(IMajorRecordGetter record);
    ITranslatedStringGetter? GetBookText(IMajorRecordGetter record);
    ITranslatedStringGetter? GetMessageDescription(IMajorRecordGetter record);
    ITranslatedStringGetter? GetDialogTopicName(IMajorRecordGetter record);
    ITranslatedStringGetter? GetDialogResponsesPrompt(IMajorRecordGetter record);
}
