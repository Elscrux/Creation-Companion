using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class ActorDialogue : AStoryManagerEvent {
    public enum ActorDialogueEvent : ushort {
        Location = 0x314C,
        Actor1 = 0x3152,
        Actor2 = 0x3252,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<ActorDialogueEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.ADIA;
}
