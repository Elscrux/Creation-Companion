using Mutagen.Bethesda.Skyrim;
namespace DialogueExporter.Services.VoiceSheets;

public record ExportLine(
    string VoiceType,
    IDialogResponseGetter Response,
    IDialogResponsesGetter Responses,
    IDialogTopicGetter Topic,
    IDialogBranchGetter? Branch,
    IQuestGetter Quest,
    INpcGetter? Npc,
    ISceneGetter? Scene,
    ISceneActionGetter? Action,
    string Speaker,
    string Context,
    string Line,
    string VaNote,
    string Emotion,
    string Path);
