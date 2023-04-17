using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class PlayerActionEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<PlayerAction>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function
        is Condition.Function.GetPlayerAction
        or Condition.Function.GetLastPlayerAction;
}
