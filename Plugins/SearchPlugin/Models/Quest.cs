using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace SearchPlugin.Models;

public class Quest : TextSearcher<ISkyrimMod, ISkyrimModGetter, IQuest, IQuestGetter> {
    public override string SearcherName => "Quest";

    protected override IEnumerable<string?> GetText(IQuestGetter record) {
        yield return record.Name?.String;
        yield return record.Description?.String;

        foreach (var objective in record.Objectives) yield return objective.DisplayText?.String;

        foreach (var stage in record.Stages) {
            foreach (var log in stage.LogEntries) {
                yield return log.Entry?.String;
            }
        }
    }

    protected override void ReplaceText(IQuest record, string oldText, string newText, StringComparison comparison) {
        if (oldText.Equals(record.Name?.String, comparison)) {
            record.Name = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }
        if (oldText.Equals(record.Description?.String, comparison)) {
            record.Description = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }

        foreach (var objective in record.Objectives.Where(objective => oldText.Equals(objective.DisplayText?.String, comparison))) {
            objective.DisplayText = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }

        var questLogEntries = record.Stages
            .SelectMany(x => x.LogEntries)
            .Where(logEntry => oldText.Equals(logEntry.Entry?.String, comparison));

        foreach (var questLogEntry in questLogEntries) {
            questLogEntry.Entry = new TranslatedString(TranslatedString.DefaultLanguage, newText);
        }
    }
}
