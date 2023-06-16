using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class Intimidate : AStoryManagerEvent {
    private enum IntimidateEvent : ushort {
        Actor = 0x3152,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<IntimidateEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.INTM;
}
