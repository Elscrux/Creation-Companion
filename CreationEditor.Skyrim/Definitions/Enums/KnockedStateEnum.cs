using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class KnockedStateEnum : IConditionValueEnums {
    private enum KnockedState {
        NotAnActor = -1,
        Normal = 0,
        Explode = 1,
        ExplodeLeadIn = 2,
        KnockedOut = 3,
        KnockedOutLeadIn = 4,
        Queued = 5,
        GetUp = 6,
        Down = 7
    }

    public IList<Enum> Enums { get; } = Enum.GetValues<KnockedState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetKnockedStateEnum;
}
