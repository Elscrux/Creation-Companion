using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class NewVoicePower : AStoryManagerEvent {
    private enum NewVoicePowerEvent : ushort {
        Actor = 0x3152,
        VoicePower = 0x3146,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<NewVoicePowerEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.NVPE;
}
