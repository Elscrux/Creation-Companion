using Mutagen.Bethesda.Strings;
namespace CreationEditor;

public static class TranslatedStringExtension {
    public static string? GetLanguageStringOrDefault(this TranslatedString translatedString, Language language) {
        return translatedString.TryLookup(language, out var languageString)
            ? languageString
            : translatedString.String;
    }
}
