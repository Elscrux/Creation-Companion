using Mutagen.Bethesda.Plugins;
namespace ModCleaner.Models.FeatureFlag;

public sealed record FeatureFlag(
    string Name,
    ModKey ModKey,
    List<WorldspaceRegions> AllowedRegions,
    List<FormLinkInformation> EssentialRecords) {
    public bool Equals(FeatureFlag? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override int GetHashCode() => HashCode.Combine(Name);
}
