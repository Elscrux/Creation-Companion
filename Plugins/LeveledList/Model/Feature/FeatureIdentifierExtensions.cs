using CreationEditor.Services.Filter;
namespace LeveledList.Model.Feature;

public static class FeatureIdentifierExtensions {
    private static readonly WildcardSearchFilter WildcardSearchFilter = new();

    extension(FeatureIdentifier featureIdentifier) {
        public bool FeatureIdentifierMatches(string? other, IReadOnlyDictionary<TierIdentifier, TierIdentifier> tierAliases) {
            if (other is null) return false;

            if (tierAliases.TryGetValue(featureIdentifier, out var original)) {
                var originalMatches = WildcardSearchFilter.Filter(other, original);
                if (originalMatches) return true;
            }

            if (tierAliases.TryGetValue(other, out var otherOriginal)) {
                var otherOriginalMatches = WildcardSearchFilter.Filter(otherOriginal, featureIdentifier);
                if (otherOriginalMatches) return true;
            }

            return WildcardSearchFilter.Filter(other, featureIdentifier);
        }

        public bool FeatureIdentifierEquals(string? other, IReadOnlyDictionary<TierIdentifier, TierIdentifier> tierAliases) {
            if (other is null) return false;

            if (int.TryParse(featureIdentifier, out var idInt) &&
                int.TryParse(other, out var otherInt)) {
                return idInt == otherInt;
            }

            if (tierAliases.TryGetValue(featureIdentifier, out var original)) {
                var originalMatches = string.Equals(original, other, StringComparison.OrdinalIgnoreCase);
                if (originalMatches) return true;
            }

            if (tierAliases.TryGetValue(other, out var otherOriginal)) {
                var otherOriginalMatches = string.Equals(otherOriginal, featureIdentifier, StringComparison.OrdinalIgnoreCase);
                if (otherOriginalMatches) return true;
            }

            return string.Equals(featureIdentifier, other, StringComparison.OrdinalIgnoreCase);
        }
    }
}
