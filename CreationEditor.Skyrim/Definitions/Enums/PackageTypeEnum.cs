using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class PackageTypeEnum : IConditionValueEnums {
    private enum PackageType {
        Invalid = -1,
        Find = 0,
        Follow = 1,
        Escort = 2,
        Eat = 3,
        Sleep = 4,
        Wander = 5,
        Travel = 6,
        Accompany = 7,
        UseItemAt = 8,
        Ambush = 9,
        FleeNotCombat = 10,
        UseMagic = 11,
        Sandbox = 12,
        Patrol = 13,
        Guard = 14,
        Dialogue = 15,
        UseWeapon = 16,
        Find2 = 17,
        Package = 18,
        PackageTemplate = 19,
        Activate = 20,
        Alarm = 21,
        Flee = 22,
        Trespass = 23,
        Spectator = 24,
        ReactToDead = 25,
        GetUpFromChair = 26,
        DoNothing = 27,
        InGameDialogue = 28,
        Surface = 29,
        SearchForAttacker = 30,
        AvoidPlayer = 31,
    }

    public IList<Enum> Enums { get; } = Enum.GetValues<PackageType>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetCurrentAIPackage;
}
