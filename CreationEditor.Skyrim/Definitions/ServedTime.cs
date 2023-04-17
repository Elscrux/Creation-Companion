using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class ServedTime : AStoryManagerEvent {
    private enum ServedTimeEvent : ushort {
        Location = 0x314C,
        CrimeGroup = 0x3146,
        CrimeGold = 0x3156,
        DaysJail = 0x3256,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<ServedTimeEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.STIJ;
}
