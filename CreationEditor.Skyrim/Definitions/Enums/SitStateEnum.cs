using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class SitStateEnum : IConditionValueEnums {
    private enum SitState {
        NotSitting = 0,
        LoadingSittingIdle = 1,
        GettingReadyToSit = 2,
        IsSitting = 3,
        GettingReadyToStandUp = 4,
        LoadingHorseMountIdle = 11,
        GettingReadyToMount = 12,
        IsSittingOnHorse = 13,
        GettingReadyToDismount = 14,
    }

    public IList<Enum> Enums { get; } = Enum.GetValues<SitState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetSitting;
}
