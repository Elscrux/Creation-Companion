using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class SkillIncrease : AStoryManagerEvent {
    public enum SkillIncreaseEvent : ushort {
        Skill = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<SkillIncreaseEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.SKIL;
}
