using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace SearchPlugin.Models;

public class Activator : TextSearcher<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter> {
    public override string SearcherName => "Activator";

    protected override IEnumerable<string?> GetText(IActivatorGetter record) {
        yield return record.ActivateTextOverride?.String;
    }

    protected override void ReplaceText(IActivator record, string oldText, string newText, StringComparison comparison) {
        if (record.ActivateTextOverride is not null && oldText.Equals(record.ActivateTextOverride.String, comparison)) {
            record.ActivateTextOverride.String = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }
    }
}
