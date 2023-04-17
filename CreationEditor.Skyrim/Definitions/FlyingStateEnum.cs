using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class FlyingStateEnum : IConditionValueEnums {
    private enum FlyingState {
        None,
        Takeoff,
        Cruising,
        Hovering,
        Landing
    }

    public IList<Enum> Enums { get; set; } = Enum.GetValues<FlyingState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetCurrentCastingType;
}
