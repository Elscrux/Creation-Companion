using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class RankTypeEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<Relationship.RankType>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function
        is Condition.Function.GetRelationshipRank
        or Condition.Function.GetHighestRelationshipRank
        or Condition.Function.GetLowestRelationshipRank;
}
