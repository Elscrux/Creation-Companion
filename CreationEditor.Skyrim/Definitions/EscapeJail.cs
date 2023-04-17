using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class EscapeJail : AStoryManagerEvent {
    public enum EscapeJailEvent : ushort {
        Location = 0x314C,
        CrimeGroup = 0x3146,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<EscapeJailEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.ESJA;
}
