using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace SearchPlugin.Models;

public sealed class Book(IRecordController recordController) : TextSearcher<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(recordController) {
    public override string SearcherName => "Book";

    protected override IEnumerable<string?> GetText(IBookGetter record) {
        if (record.Description is { String: not null }) yield return record.Description.String;

        yield return record.BookText.String;
    }

    protected override void ReplaceText(IBook record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.Description?.String, comparison)) {
            record.Description = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }

        if (oldText.Equals(record.BookText.String, comparison)) {
            record.BookText = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }
    }
}
