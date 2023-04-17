using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class CrimeGold : AStoryManagerEvent {
    public enum CrimeGoldEvent : ushort {
        Victim = 0x3152,
        Criminal = 0x3252,
        CrimeFaction = 0x3146,
        GoldValue = 0x3156,
        CrimeType = 0x3256,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<CrimeGoldEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.ADCR;
}
