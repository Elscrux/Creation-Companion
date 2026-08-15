using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Skyrim;
namespace SearchPlugin.Models;

public sealed class Npc(IRecordController recordController) : TextSearcher<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(recordController) {
    public override string SearcherName => "Npc";

    protected override IEnumerable<string?> GetText(INpcGetter record) {
        // Name already handled by generic name handler
        yield return record.ShortName?.String;
    }

    protected override void ReplaceText(INpc record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.ShortName?.String, comparison)) {
            record.ShortName = newText;
        }
    }
}
