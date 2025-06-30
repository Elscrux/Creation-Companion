namespace LeveledList.Services;

public sealed record Tier(TierIdentifier TierIdentifier) {
    public override string ToString() => TierIdentifier;
}
