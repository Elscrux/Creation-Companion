using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class CombatReactionEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<CombatReaction>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function
        is Condition.Function.GetFactionRelation
        or Condition.Function.GetFactionCombatReaction;
}
