namespace LeveledList.Model.List;

public sealed record LeveledList(
    IReadOnlyList<Feature.Feature> Features,
    string EditorID,
    List<LeveledListEntry> Entries,
    float Chance,
    bool UseAll,
    bool CalculateForEach,
    bool CalculateFromAllLevels,
    bool SpecialLoot);
