using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class SleepStateEnum : IConditionValueEnums {
    private enum SleepState {
        NotSleeping = 0,
        LoadingSleepingIdle = 1,
        GettingReadyToSleep = 2,
        IsSleeping = 3,
        GettingReadyToWakeUp = 4
    }

    public IList<Enum> Enums { get; set; } = Enum.GetValues<SleepState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetSleeping;
}
