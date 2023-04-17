using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class Intimidate : AStoryManagerEvent {
    public enum IntimidateEvent : ushort {
        Actor = 0x3152,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<IntimidateEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.INTM;
}
