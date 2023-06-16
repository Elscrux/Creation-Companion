using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class CraftItem : AStoryManagerEvent {
    private enum CraftItemEvent : ushort {
        Workbench = 0x3152,
        BenchLocation = 0x314C,
        CreatedObject = 0x314F,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<CraftItemEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.CRFT;
}
