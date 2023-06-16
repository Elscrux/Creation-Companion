using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class AssaultActor : AStoryManagerEvent {
    private enum AssaultActorEvent : ushort {
        Victim = 0x3152,
        Attacker = 0x3252,
        Location = 0x314C,
        Crime = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<AssaultActorEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.ASSU;
}
