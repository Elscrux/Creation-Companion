using System.Text.RegularExpressions;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed partial class RecordGlobalVariableReferenceQuery(IMutagenCommonAspectsProvider commonAspectsProvider)
    : IReferenceQuery<IModGetter, DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IFormLinkIdentifier> {
    [GeneratedRegex(@"<Global(?:\.(?:Hour12|Minutes|Month|MonthWord|Day|WeekDay|Year|TimeSpan|Meridiem|Time))?=(\w+)>", RegexOptions.IgnoreCase)]
    public static partial Regex GlobalVariableRegex { get; }

    public string Name => "Record Global Variable Links";

    public string GetSourceName(IModGetter source) => source.ModKey.FileName;
    public IModGetter? ReferenceToSource(IFormLinkIdentifier reference) => null;

    public void FillCache(IModGetter source, DictionaryReferenceCache<string, IFormLinkIdentifier> cache) {
        foreach (var book in source.EnumerateMajorRecords(commonAspectsProvider.BookType)) {
            var bookText = commonAspectsProvider.GetBookText(book);
            if (bookText is null) continue;

            ProcessTranslatedText(bookText, book);
        }

        foreach (var message in source.EnumerateMajorRecords(commonAspectsProvider.MessageType)) {
            var messageDescription = commonAspectsProvider.GetMessageDescription(message);
            if (messageDescription is null) continue;

            ProcessTranslatedText(messageDescription, message);
        }

        foreach (var topic in source.EnumerateMajorRecords(commonAspectsProvider.DialogTopic)) {
            var name = commonAspectsProvider.GetDialogTopicName(topic);
            if (name is null) continue;

            ProcessTranslatedText(name, topic);
        }

        foreach (var responses in source.EnumerateMajorRecords(commonAspectsProvider.DialogResponses)) {
            var prompt = commonAspectsProvider.GetDialogResponsesPrompt(responses);
            if (prompt is null) continue;

            ProcessTranslatedText(prompt, responses);
        }

        void ProcessTranslatedText(ITranslatedStringGetter translatedString, IFormLinkIdentifier recordLink) {
            foreach (var (_, text) in translatedString) {
                foreach (Match match in GlobalVariableRegex.Matches(text)) {
                    var globalName = match.Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(globalName)) continue;

                    cache.Cache.GetOrAdd(globalName).Add(recordLink);
                }
            }
        }
    }

    public IEnumerable<string> ParseRecord(IMajorRecordGetter record) {
        var globalNames = ParseTranslatedString(commonAspectsProvider.GetBookText(record));
        globalNames = globalNames.Concat(ParseTranslatedString(commonAspectsProvider.GetMessageDescription(record)));
        globalNames = globalNames.Concat(ParseTranslatedString(commonAspectsProvider.GetDialogTopicName(record)));
        globalNames = globalNames.Concat(ParseTranslatedString(commonAspectsProvider.GetDialogResponsesPrompt(record)));
        return globalNames;

        IEnumerable<string> ParseTranslatedString(ITranslatedStringGetter? translatedString) {
            if (translatedString is null) yield break;

            foreach (var (_, text) in translatedString) {
                foreach (Match match in GlobalVariableRegex.Matches(text)) {
                    var globalName = match.Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(globalName)) continue;

                    yield return globalName;
                }
            }
        }
    }
}
