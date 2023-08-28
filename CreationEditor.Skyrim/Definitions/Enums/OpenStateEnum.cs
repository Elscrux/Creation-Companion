using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class OpenStateEnum : IConditionValueEnums {
    private enum OpenState {
        None,
        Open,
        Opening,
        Closed,
        Closing
    }

    public IList<Enum> Enums { get; } = Enum.GetValues<OpenState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetOpenState;
}
