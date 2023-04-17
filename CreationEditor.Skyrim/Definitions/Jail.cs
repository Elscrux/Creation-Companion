using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class Jail : AStoryManagerEvent {
    private enum JailEvent : ushort {
        Guard = 0x3152,
        CrimeGroup = 0x3146,
        Location = 0x314C,
        CrimeGold = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<JailEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.JAIL;
}
