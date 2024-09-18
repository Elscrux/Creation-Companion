using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class CombatStyleFilter : SimpleRecordFilter<ICombatStyleGetter> {
    public CombatStyleFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Melee", record => IsBiggest(record, record.EquipmentScoreMultMelee)),
        new("Ranged", record => IsBiggest(record, record.EquipmentScoreMultRanged)),
        new("Magic", record => IsBiggest(record, record.EquipmentScoreMultMagic)),
        new("Shout", record => IsBiggest(record, record.EquipmentScoreMultShout)),
        new("Staff", record => IsBiggest(record, record.EquipmentScoreMultStaff)),
        new("Unarmed", record => IsBiggest(record, record.EquipmentScoreMultUnarmed)),
    }) {}

    public static bool IsBiggest(ICombatStyleGetter combatStyle, float score) {
        float[] scores = [
            combatStyle.EquipmentScoreMultMelee,
            combatStyle.EquipmentScoreMultMagic,
            combatStyle.EquipmentScoreMultRanged,
            combatStyle.EquipmentScoreMultShout,
            combatStyle.EquipmentScoreMultStaff,
            combatStyle.EquipmentScoreMultUnarmed,
        ];

        var found = false;

        foreach (double value in scores) {
            if (score < value) return false;

            if (Math.Abs(score - value) >= 0.001) continue;

            if (!found) {
                found = true;
            } else {
                return false;
            }
        }

        return true;
    }
}
