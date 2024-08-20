using Avalonia.Data.Converters;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Converter;

public static class ToolTipConverters {
    public static readonly FuncValueConverter<CombatReaction, string> CombatReactionToString
        = new(reaction =>
            reaction switch {
                CombatReaction.Neutral =>
                    "This is how all factions relate to each other by default, even if you don't specify it. Neutrals will be attacked by Very Aggressive and Frenzied actors, and are not assisted by anyone.",
                CombatReaction.Enemy =>
                    "Enemy actors are attacked on sight by Aggressive, Very Aggressive, and Frenzied actors, and are not assisted by anyone.",
                CombatReaction.Ally =>
                    "Allies will only be attacked by Frenzied actors, and will be assisted by Helps Friends and Allies and Helps Allies actors.",
                CombatReaction.Friend => "Friends will only be attacked by Frenzied actors, and will be assisted by Helps Friends and Allies actors.",
                _ => string.Empty,
            }
        );
}
