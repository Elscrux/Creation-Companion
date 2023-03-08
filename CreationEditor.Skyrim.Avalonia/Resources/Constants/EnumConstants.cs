using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants;

public static class EnumConstants {
    public static readonly IEnumerable<SpellType> SpellTypes = Enum.GetValues<SpellType>().Except(SpellType.Addiction.AsEnumerable());

    public static readonly IEnumerable<CombatReaction> CombatReactions = new[] {
        CombatReaction.Ally,
        CombatReaction.Friend,
        CombatReaction.Neutral,
        CombatReaction.Enemy,
    };
}
