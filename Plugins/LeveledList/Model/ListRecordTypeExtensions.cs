using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Model;

public static class ListRecordTypeExtensions {
    public static IEnumerable<Type> GetRecordTypes(this ListRecordType type) {
        return type switch {
            ListRecordType.Armor => new[] { typeof(IArmorGetter) },
            ListRecordType.Weapon => new[] { typeof(IWeaponGetter), typeof(IAmmunitionGetter) },
            ListRecordType.Poison => new[] { typeof(IIngestibleGetter) },
            ListRecordType.Potion => new[] { typeof(IIngestibleGetter) },
            ListRecordType.SpellTome => new[] { typeof(IBookGetter) },
            ListRecordType.Staff => new[] { typeof(IWeaponGetter) },
            ListRecordType.Ingredient => new[] { typeof(IIngredientGetter) },
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
