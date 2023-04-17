using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class PlayerCured : AStoryManagerEvent {
    private enum PlayerCuredEvent : ushort {
        Infection = 0x3146,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<PlayerCuredEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.CURE;
}
