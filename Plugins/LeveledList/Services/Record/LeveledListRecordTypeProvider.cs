using CreationEditor.Skyrim;
using LeveledList.Model;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace LeveledList.Services.Record;

public class LeveledListRecordTypeProvider(ITierController tierController) : ILeveledListRecordTypeProvider {
    private static IEnumerable<IMajorRecordGetter> GetRecordsOnly(IModGetter mod, ListRecordType type) {
        return type switch {
            ListRecordType.Armor => mod.EnumerateMajorRecords<IArmorGetter>(),
            ListRecordType.Weapon => mod.EnumerateMajorRecords<IWeaponGetter>()
                .Where(w => w.Data is not { AnimationType: WeaponAnimationType.Staff })
                .Concat<IMajorRecordGetter>(mod.EnumerateMajorRecords<IAmmunitionGetter>()),
            ListRecordType.Poison => mod.EnumerateMajorRecords<IIngestibleGetter>()
                .Where(i => i.IsPoison()),
            ListRecordType.Potion => mod.EnumerateMajorRecords<IIngestibleGetter>()
                .Where(i => i.IsPotion()),
            ListRecordType.SpellTome => mod.EnumerateMajorRecords<IBookGetter>()
                .Where(b => b.Teaches is IBookSpellGetter),
            ListRecordType.Staff => mod.EnumerateMajorRecords<IWeaponGetter>()
                .Where(w => w.Data is { AnimationType: WeaponAnimationType.Staff }),
            ListRecordType.Ingredient => mod.EnumerateMajorRecords<IIngredientGetter>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public ListRecordType? GetListRecordType(IMajorRecordGetter record) {
        if (record is IArmorGetter) return ListRecordType.Armor;
        if (record is IWeaponGetter weapon) {
            return weapon.Data is { AnimationType: WeaponAnimationType.Staff }
                ? ListRecordType.Staff
                : ListRecordType.Weapon;
        }
        if (record is IAmmunitionGetter) return ListRecordType.Weapon; // Ammunition is treated as a weapon
        if (record is IIngestibleGetter ingestible) {
            if (ingestible.IsPoison()) return ListRecordType.Poison;
            if (ingestible.IsPotion()) return ListRecordType.Potion;
        }
        if (record is IBookGetter book) {
            return book.Teaches is IBookSpellGetter ? ListRecordType.SpellTome : ListRecordType.Staff;
        }
        if (record is IIngredientGetter) return ListRecordType.Ingredient;

        return null;
    }

    public IEnumerable<RecordWithTier> GetRecords(IModGetter mod, ListRecordType type) {
        return GetRecordsOnly(mod, type)
            .Select(record => {
                var tier = tierController.GetRecordTier(record);
                if (tier is null) return null;

                return new RecordWithTier(record, tier);
            })
            .WhereNotNull();
    }
}
