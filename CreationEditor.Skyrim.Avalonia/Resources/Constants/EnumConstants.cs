using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants; 

public static class EnumConstants {
    public static readonly IEnumerable<CombatReaction> CombatReactions = new[] {
        CombatReaction.Ally,
        CombatReaction.Friend,
        CombatReaction.Neutral,
        CombatReaction.Enemy,
    };
}
