namespace LeveledList.Model.Tier;

public sealed record TierDefinitions(
    Dictionary<TierIdentifier, TierData> Tiers);

public sealed record TierData(
    List<int> Levels);
