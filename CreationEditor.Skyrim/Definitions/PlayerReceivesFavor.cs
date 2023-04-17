using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class PlayerReceivesFavor : AStoryManagerEvent {
    private enum PlayerReceivesFavorEvent : ushort {
        Actor = 0x3152,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<PlayerReceivesFavorEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.PRFV;
}
