using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class PayFine : AStoryManagerEvent {
    private enum PayFineEvent : ushort {
        Criminal = 0x3152,
        Guard = 0x3252,
        CrimeGroup = 0x3146,
        CrimeGold = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<PayFineEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.PFIN;
}
