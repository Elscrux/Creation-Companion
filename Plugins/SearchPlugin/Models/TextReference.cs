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
}
