using CreationEditor.Services.Filter;
namespace LeveledList.Model.Feature;

public static class FeatureIdentifierExtensions {
    private static readonly WildcardSearchFilter WildcardSearchFilter = new();

    public static bool FeatureIdentifierEquals(this FeatureIdentifier featureIdentifier, string? other) {
        if (other is null) return false;

        return WildcardSearchFilter.Filter(other, featureIdentifier);
    }
}
