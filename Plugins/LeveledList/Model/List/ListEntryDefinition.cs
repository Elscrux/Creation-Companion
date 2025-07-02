namespace LeveledList.Model.List;

public record ListEntryDefinition(
    short Level,
    short Count = 1,
    int Amount = 1,
    short Interval = 0,
    int EnchantmentLevel = 0
) {
    public ListEntryDefinition() : this(1) {}

    public IEnumerable<LeveledListEntry> GetEntries(LeveledListEntryItem item) {
        var currentLevel = Level;
        for (var i = 0; i < Amount; i++) {
            yield return new LeveledListEntry(
                item,
                currentLevel,
                Count
            );
            currentLevel += Interval;
        }
    }
}
