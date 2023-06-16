using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class PlayerInfected : AStoryManagerEvent {
    private enum PlayerInfectedEvent : ushort {
        TransmittingActor = 0x3152,
        Infection = 0x3146,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<PlayerInfectedEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.INFC;
}
