using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class CombatStateEnum : IConditionValueEnums {
    private enum CombatState {
        NotInCombat,
        InCombat,
        Searching,
    }

    public IList<Enum> Enums { get; set; } = Enum.GetValues<CombatState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetCombatState;
}
