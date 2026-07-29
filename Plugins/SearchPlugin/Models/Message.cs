using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace SearchPlugin.Models;

public class Message(IRecordController recordController) : TextSearcher<ISkyrimMod, ISkyrimModGetter, IMessage, IMessageGetter>(recordController) {
    public override string SearcherName => "Message";

    protected override IEnumerable<string?> GetText(IMessageGetter record) {
        yield return record.Description.String;
    }

    protected override void ReplaceText(IMessage record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.Description.String, comparison)) {
            record.Description = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }
    }
}
