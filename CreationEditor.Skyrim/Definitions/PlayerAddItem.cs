using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class PlayerAddItem : AStoryManagerEvent {
    private enum PlayerAddItemEvent : ushort {
        OwnerRef = 0x3152,
        OriginalContainer = 0x3252,
        Location = 0x314C,
        ObjectForm = 0x3146,
        AcquireType = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<PlayerAddItemEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.AIPL;
}
