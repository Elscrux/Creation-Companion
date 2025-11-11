using CreationEditor.Services.Filter;
namespace LeveledList.Model.Feature;

public static class FeatureIdentifierExtensions {
    private static readonly WildcardSearchFilter WildcardSearchFilter = new();

    extension(FeatureIdentifier featureIdentifier) {
        public bool FeatureIdentifierMatches(string? other) {
            if (other is null) return false;

            return WildcardSearchFilter.Filter(other, featureIdentifier);
        }
        public bool FeatureIdentifierEquals(string? other) {
            if (other is null) return false;

            if (int.TryParse(featureIdentifier, out var idInt) &&
                int.TryParse(other, out var otherInt)) {
                return idInt == otherInt;
            }

            return string.Equals(featureIdentifier, other, StringComparison.OrdinalIgnoreCase);
        }
    }

}
