using Mutagen.Bethesda.Strings;
namespace CreationEditor;

public static class TranslatedStringExtension {
    extension(TranslatedString translatedString) {
        public string? GetLanguageStringOrDefault(Language language) {
            return translatedString.TryLookup(language, out var languageString)
                ? languageString
                : translatedString.String;
        }
    }
}
