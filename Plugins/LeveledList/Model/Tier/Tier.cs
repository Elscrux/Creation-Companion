namespace LeveledList.Model.Tier;

public sealed record Tier(TierIdentifier TierIdentifier) {
    public override string ToString() => TierIdentifier;
}
