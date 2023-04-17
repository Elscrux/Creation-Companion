using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class ChangeRelationshipRank : AStoryManagerEvent {
    public enum ChangeRelationshipRankEvent : ushort {
        NPC1 = 0x3152,
        NPC2 = 0x3252,
        OldRelationship = 0x3156,
        NewRelationship = 0x3256,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<ChangeRelationshipRankEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.CHRR;
}
