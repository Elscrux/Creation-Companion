using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Skyrim;
namespace SearchPlugin.Models;

public sealed class Name(IRecordController recordController) : TextSearcher<ISkyrimMod, ISkyrimModGetter, INamed, INamedGetter>(recordController) {
    public override string SearcherName => "Name";

    protected override IEnumerable<string?> GetText(INamedGetter record) {
        yield return record.Name;
    }

    protected override void ReplaceText(INamed record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.Name, comparison)) {
            record.Name = newText;
        }
    }
}
