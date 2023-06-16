using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class Script : AStoryManagerEvent {
    private enum ScriptEvent : ushort {
        Keyword = 0x314B,
        Location = 0x314C,
        Ref1 = 0x3152,
        Ref2 = 0x3252,
        Value1 = 0x3156,
        Value2 = 0x3256,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<ScriptEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.SCPT;
}
