namespace LeveledList.Model.Tier;

public sealed record TierDefinitions(
    Dictionary<TierIdentifier, TierData> Tiers,
    Dictionary<TierIdentifier, TierIdentifier>? TierAliases);

public sealed record TierData(
    List<int> Levels);
