using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class Flatter : AStoryManagerEvent {
    public enum FlatterEvent : ushort {
        Actor = 0x3152,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<FlatterEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.FLAT;
}
