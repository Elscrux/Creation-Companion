using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class Flatter : AStoryManagerEvent {
    private enum FlatterEvent : ushort {
        Actor = 0x3152,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<FlatterEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.FLAT;
}
