using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class EscapeJail : AStoryManagerEvent {
    private enum EscapeJailEvent : ushort {
        Location = 0x314C,
        CrimeGroup = 0x3146,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<EscapeJailEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.ESJA;
}
