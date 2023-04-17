using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class KillActor : AStoryManagerEvent {
    public enum KillActorEvent : ushort {
        Victim = 0x3152,
        Killer = 0x3252,
        Location = 0x314C,
        CrimeStatus = 0x3156,
        RelationshipRankToKillerBeforeDeath = 0x3256,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<KillActorEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.KILL;
}
