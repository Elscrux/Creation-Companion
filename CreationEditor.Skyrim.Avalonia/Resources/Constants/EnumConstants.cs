using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants;

public static class EnumConstants {
    public static readonly IEnumerable<SpellType> SpellTypes
        = Enum.GetValues<SpellType>().Except(SpellType.Addiction.AsEnumerable());

    public static readonly CombatReaction[] CombatReactions = {
        CombatReaction.Ally,
        CombatReaction.Friend,
        CombatReaction.Neutral,
        CombatReaction.Enemy,
    };

    public static readonly IEnumerable<CompareOperator> CompareOperatorTypes
        = Enum.GetValues<CompareOperator>();

    public static readonly string[] CompareOperatorTypesString = {
        "==",
        "!=",
        ">",
        ">=",
        "<",
        "<=",
    };

    public static readonly string[] AndOr = {
        "And",
        "Or",
    };

    public static readonly IEnumerable<TargetObjectType> ObjectTypes
        = Enum.GetValues<TargetObjectType>();

    public static readonly IEnumerable<QuestAlias.TypeEnum> ReferenceAliasTypes
        = QuestAlias.TypeEnum.Reference.AsEnumerable();

    public static readonly IEnumerable<QuestAlias.TypeEnum> LocationAliasTypes
        = QuestAlias.TypeEnum.Location.AsEnumerable();
}
