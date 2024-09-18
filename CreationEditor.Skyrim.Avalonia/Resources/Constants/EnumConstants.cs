using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants;

public static class EnumConstants {
    public static readonly IEnumerable<SpellType> SpellTypes
        = Enum.GetValues<SpellType>().Except(SpellType.Addiction.AsEnumerable());

    public static readonly CombatReaction[] CombatReactions = [
        CombatReaction.Ally,
        CombatReaction.Friend,
        CombatReaction.Neutral,
        CombatReaction.Enemy,
    ];

    public static readonly IEnumerable<CompareOperator> CompareOperatorTypes
        = Enum.GetValues<CompareOperator>();

    public static readonly IEnumerable<Skill> Skills
        = Enum.GetValues<Skill>().OrderBy(x => x.ToString()).ToList();

    public static readonly IEnumerable<Language> Languages = [
        Language.English,
        Language.French,
        Language.Italian,
        Language.German,
        Language.Spanish,
        Language.Russian,
        Language.Polish,
        Language.Japanese,
        Language.Chinese,
    ];

    public static readonly string[] CompareOperatorTypesString = [
        "==",
        "!=",
        ">",
        ">=",
        "<",
        "<=",
    ];

    public static readonly string[] AndOr = [
        "And",
        "Or",
    ];

    public static readonly IEnumerable<TargetObjectType> ObjectTypes
        = Enum.GetValues<TargetObjectType>();

    public static readonly IEnumerable<QuestAlias.TypeEnum> ReferenceAliasTypes
        = QuestAlias.TypeEnum.Reference.AsEnumerable();

    public static readonly IEnumerable<QuestAlias.TypeEnum> LocationAliasTypes
        = QuestAlias.TypeEnum.Location.AsEnumerable();
}
