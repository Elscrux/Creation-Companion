using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace SearchPlugin.Models;

public sealed class EditorID(IRecordController recordController) : TextSearcher<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(recordController) {
    public override string SearcherName => "EditorID";

    protected override IEnumerable<string?> GetText(IMajorRecordGetter record) {
        yield return record.EditorID;
    }

    protected override void ReplaceText(IMajorRecord record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.EditorID, comparison)) {
            record.EditorID = newText;
        }
    }
}
