using LeveledList.Model.Tier;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace LeveledList.Services.Record;

public class RecordTypeProvider(ITierController tierController) : IRecordTypeProvider {
    private static IEnumerable<IMajorRecordGetter> GetRecordsOnly(IModGetter mod, string type) {
        return type switch {
            "armor" => mod.EnumerateMajorRecords<IArmorGetter>(),
            "weapon" => mod.EnumerateMajorRecords<IWeaponGetter>(),
            "poison" => mod.EnumerateMajorRecords<IIngestibleGetter>()
                .Where(i => i.Flags.HasFlag(Ingestible.Flag.Poison)),
            "potion" => mod.EnumerateMajorRecords<IIngestibleGetter>()
                .Where(i => (i.Flags & (Ingestible.Flag.Poison | Ingestible.Flag.FoodItem | Ingestible.Flag.Medicine)) == 0),
            "spelltome" => mod.EnumerateMajorRecords<IBookGetter>()
                .Where(b => b.Teaches is IBookSpellGetter),
            "staff" => mod.EnumerateMajorRecords<IWeaponGetter>()
                .Where(w => w.Data is { AnimationType: WeaponAnimationType.Staff }),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public IEnumerable<RecordWithTier> GetRecords(IModGetter mod, string type) {
        return GetRecordsOnly(mod, type)
            .Select(record => {
                var tier = tierController.GetRecordTier(record);
                if (tier is null) return null;

                return new RecordWithTier(record, tier);
            })
            .WhereNotNull();
    }
}
