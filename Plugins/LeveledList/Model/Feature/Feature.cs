namespace LeveledList.Model.Feature;

public sealed record Feature(FeatureWildcard Wildcard, object Key) {
    public override string ToString() {
        return $"{Wildcard.Identifier}={Key}";
    }
};
