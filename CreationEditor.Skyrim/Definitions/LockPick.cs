using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class LockPick : AStoryManagerEvent {
    private enum LockPickEvent : ushort {
        Actor = 0x3152,
        LockObject = 0x3252,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<LockPickEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.LOCK;
}
