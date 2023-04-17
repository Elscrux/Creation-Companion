using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class IncreaseLevel : AStoryManagerEvent {
    public enum IncreaseLevelEvent : ushort {
        NewLevel = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<IncreaseLevelEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.LEVL;
}
