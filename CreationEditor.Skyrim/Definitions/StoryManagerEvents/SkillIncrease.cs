using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class SkillIncrease : AStoryManagerEvent {
    private enum SkillIncreaseEvent : ushort {
        Skill = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<SkillIncreaseEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.SKIL;
}
