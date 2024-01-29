using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace SearchPlugin.Models;

public class Topic : TextSearcher<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter> {
    public override string SearcherName => "Dialogue";

    protected override IEnumerable<string?> GetText(IDialogTopicGetter record) {
        yield return record.Name?.String;

        foreach (var dialog in record.Responses) {
            yield return dialog.Prompt?.String;

            foreach (var response in dialog.Responses) yield return response.Text.String;
        }
    }

    protected override void ReplaceText(IDialogTopic record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.Name?.String, comparison)) record.Name = new TranslatedString(TranslatedString.DefaultLanguage, newText);

        foreach (var resp in record.Responses) {
            if (oldText.Equals(resp.Prompt?.String, comparison)) resp.Prompt = new TranslatedString(TranslatedString.DefaultLanguage, newText);

            foreach (var response in resp.Responses.Where(response => oldText.Equals(response.Text.String, comparison))) {
                response.Text = new TranslatedString(TranslatedString.DefaultLanguage, newText);
            }
        }
    }
}
