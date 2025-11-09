namespace LeveledList.Model.Feature;

public record Feature(FeatureWildcard Wildcard, object Key) {
    public override string ToString() {
        return $"{Wildcard.Identifier}={Key}";
    }
};
