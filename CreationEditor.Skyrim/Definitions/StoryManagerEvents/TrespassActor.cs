using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class TrespassActor : AStoryManagerEvent {
    private enum TrespassActorEvent : ushort {
        Victim = 0x3152,
        Trespasser = 0x3252,
        Location = 0x314C,
        Crime = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<TrespassActorEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.TRES;
}
