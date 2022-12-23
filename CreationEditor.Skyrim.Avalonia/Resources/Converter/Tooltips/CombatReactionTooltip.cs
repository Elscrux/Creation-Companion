using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Converter.Tooltips; 

public class CombatReactionTooltip : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is CombatReaction combatReaction) {
            return combatReaction switch {
                CombatReaction.Neutral => "This is how all factions relate to each other by default, even if you don't specify it. Neutrals will be attacked by Very Aggressive and Frenzied actors, and are not assisted by anyone.",
                CombatReaction.Enemy => "Enemy actors are attacked on sight by Aggressive, Very Aggressive, and Frenzied actors, and are not assisted by anyone.",
                CombatReaction.Ally => "Allies will only be attacked by Frenzied actors, and will be assisted by Helps Friends and Allies and Helps Allies actors.",
                CombatReaction.Friend => "Friends will only be attacked by Frenzied actors, and will be assisted by Helps Friends and Allies actors.",
                _ => string.Empty
            };
        }

        return string.Empty;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
