using CreationEditor;
using Mutagen.Bethesda.Plugins.Records;
namespace SearchPlugin.Models;

public sealed record TextReference(
    ITextSearcher TextSearcher,
    IMajorRecordQueryableGetter Record,
    TextDiff Diff) {

    public override string? ToString() {
        return Record.GetName();
    }

    public bool Equals(TextReference? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Record.Equals(other.Record) && Diff.Equals(other.Diff);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Record, Diff);
    }
}
