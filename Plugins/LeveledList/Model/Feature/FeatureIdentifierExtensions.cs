namespace LeveledList.Model.Feature;

public static class FeatureIdentifierExtensions {
    public static bool FeatureIdentifierEquals(this FeatureIdentifier featureIdentifier, string? other) {
        return StringComparer.OrdinalIgnoreCase.Equals(featureIdentifier, other);
    }
}
