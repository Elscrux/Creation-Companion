using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class PlayerActivateActor : AStoryManagerEvent {
    private enum PlayerActivateActorEvent : ushort {
        Location = 0x314C,
        Actor = 0x3152,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<PlayerActivateActorEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.AFAV;
}
