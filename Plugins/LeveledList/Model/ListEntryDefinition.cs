using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Model;

public record ListEntryDefinition(
    short Level,
    short Count = 1,
    int Amount = 1,
    short Interval = 0,
    int EnchantmentLevel = 0
) {
    public ListEntryDefinition() : this(1) {}

    public IEnumerable<LeveledItemEntry> GetEntries(IMajorRecordGetter item) {
        var currentLevel = Level;
        for (var i = 0; i < Amount; i++) {
            yield return new LeveledItemEntry {
                Data = new LeveledItemEntryData {
                    Reference = new FormLink<IItemGetter>(item.FormKey),
                    Level = currentLevel,
                    Count = Count,
                }
            };
            currentLevel += Interval;
        }
    }
}
