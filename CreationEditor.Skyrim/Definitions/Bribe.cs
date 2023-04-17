using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class Bribe : AStoryManagerEvent {
    private enum BribeEvent : ushort {
        Actor = 0x3152,
        GoldValue = 0x3156,
    }
    public override IList<Enum> Enums { get; } = Enum.GetValues<BribeEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.BRIB;
}
