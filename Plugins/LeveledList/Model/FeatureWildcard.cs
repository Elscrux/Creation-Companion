using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Model;

public record FeatureWildcard(FeatureWildcardIdentifier Identifier, Func<IMajorRecordGetter, object?> Selector) {
    public virtual bool Equals(FeatureWildcard? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Identifier == other.Identifier;
    }
    public override int GetHashCode() => Identifier.GetHashCode();
}
