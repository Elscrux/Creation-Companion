using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class ActorHello : AStoryManagerEvent {
    private enum ActorHelloEvent : ushort {
        Location = 0x314C,
        Actor1 = 0x3152,
        Actor2 = 0x3252,
    }


    public override IList<Enum> Enums { get; } = Enum.GetValues<ActorHelloEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.AHEL;
}
