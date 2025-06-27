using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services;

public class RecordTypeProvider(IModGetter mod) {
    public IEnumerable<IMajorRecordGetter> GetRecords(string type) {
        return type switch {
            "armor" => mod.EnumerateMajorRecords<IArmorGetter>(),
            "weapon" => mod.EnumerateMajorRecords<IWeaponGetter>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
