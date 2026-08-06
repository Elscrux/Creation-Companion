using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace SearchPlugin.Models;

public sealed class LoadScreen(IRecordController recordController) : TextSearcher<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(recordController) {
    public override string SearcherName => "LoadScreen";

    protected override IEnumerable<string?> GetText(ILoadScreenGetter record) {
        yield return record.Description.String;
    }
    protected override void ReplaceText(ILoadScreen record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.Description.String, comparison)) {
            record.Description = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }
    }
}
