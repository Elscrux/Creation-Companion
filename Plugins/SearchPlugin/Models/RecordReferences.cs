using CreationEditor;
using Mutagen.Bethesda.Plugins.Records;
namespace SearchPlugin.Models;

public sealed record RecordReferences<TMod, TModGetter>(
    ITextSearcher<TMod, TModGetter> TextSearcher,
    IMajorRecordQueryableGetter Record,
    TextDiff Diff)
    where TModGetter : class, IModGetter
    where TMod : class, TModGetter, IMod {

    public override string? ToString() {
        return Record.GetName();
    }
}
