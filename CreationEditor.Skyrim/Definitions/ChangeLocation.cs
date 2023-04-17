using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class ChangeLocation : AStoryManagerEvent {
    private enum ChangeLocationEvent : ushort {
        Actor = 0x3152,
        OldLocation = 0x314C,
        NewLocation = 0x324C,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<ChangeLocationEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.CLOC;
}
