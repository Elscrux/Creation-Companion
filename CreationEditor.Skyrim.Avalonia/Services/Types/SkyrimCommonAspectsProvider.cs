using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace CreationEditor.Skyrim.Avalonia.Services.Types;

public sealed class SkyrimCommonAspectsProvider : IMutagenCommonAspectsProvider {
    public Type AddonNodeRecordType { get; } = typeof(IAddonNodeGetter);
    public Type SoundDescriptorRecordType { get; } = typeof(ISoundDescriptorGetter);
    public Type BookType { get; } = typeof(IBookGetter);
    public Type MessageType { get; } = typeof(IMessageGetter);
    public Type DialogTopic { get; } = typeof(IDialogTopicGetter);
    public Type DialogResponses { get; } = typeof(IDialogResponsesGetter);

    public int? GetAddonNodeIndex(IMajorRecordGetter record) {
        if (record is IAddonNodeGetter addonNode) {
            return addonNode.NodeIndex;
        }

        return null;
    }

    public ITranslatedStringGetter? GetBookText(IMajorRecordGetter record) {
        if (record is IBookGetter book) {
            return book.BookText;
        }

        return null;
    }

    public ITranslatedStringGetter? GetMessageDescription(IMajorRecordGetter record) {
        if (record is IMessageGetter message) {
            return message.Description;
        }

        return null;
    }

    public ITranslatedStringGetter? GetDialogTopicName(IMajorRecordGetter record) {
        if (record is IDialogTopicGetter dialogTopic) {
            return dialogTopic.Name;
        }

        return null;
    }

    public ITranslatedStringGetter? GetDialogResponsesPrompt(IMajorRecordGetter record) {
        if (record is IDialogResponsesGetter dialogResponses) {
            return dialogResponses.Prompt;
        }

        return null;
    }
}
