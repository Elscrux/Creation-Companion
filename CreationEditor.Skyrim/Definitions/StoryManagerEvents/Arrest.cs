using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class Arrest : AStoryManagerEvent {
    private enum ArrestEvent : ushort {
        ArrestingGuard = 0x3152,
        Criminal = 0x3252,
        Location = 0x314C,
        CrimeType = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<ArrestEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.ARRT;
}
