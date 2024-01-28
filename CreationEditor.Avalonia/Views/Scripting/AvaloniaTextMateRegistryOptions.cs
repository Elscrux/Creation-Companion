using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Platform;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;
namespace CreationEditor.Avalonia.Views.Scripting;

public sealed class AvaloniaTextMateRegistryOptions : IRegistryOptions {
    private const string BasePath = "avares://CreationEditor.Avalonia/Assets";

    private readonly ThemeName _defaultTheme;
    private readonly List<GrammarDefinition> _availableGrammarDefinitions = [];
    private readonly List<IRawGrammar> _availableGrammars = [];

    public AvaloniaTextMateRegistryOptions(ThemeName defaultTheme, params string[] grammars) {
        _defaultTheme = defaultTheme;
        InitGrammars(grammars);
    }

    private void InitGrammars(params string[] grammars) {
        var definitionOptions = new JsonSerializerOptions {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            IncludeFields = false,
            WriteIndented = false
        };
        var languageOptions = new JsonSerializerOptions {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        foreach (var grammarName in grammars) {
            using var definitionStream = AssetLoader.Open(new Uri($"{BasePath}/{grammarName}/package.json"));

            var grammarDefinition = JsonSerializer.Deserialize<GrammarDefinition>(definitionStream, definitionOptions)
             ?? throw new JsonException("Failed to deserialize grammar definition");

            foreach (var language in grammarDefinition.Contributes.Languages) {
                var grammarFile = language.ConfigurationFile.TrimStart('.').TrimStart('/');
                using var languageStream = AssetLoader.Open(new Uri($"{BasePath}/{grammarName}/{grammarFile}"));
                language.Configuration = JsonSerializer.Deserialize<LanguageConfiguration>(languageStream, languageOptions);
            }

            foreach (var grammar in grammarDefinition.Contributes.Grammars) {
                var grammarUri = new Uri($"{BasePath}/{grammarName}/" + grammar.Path.TrimStart('.').TrimStart('/'));
                using var grammarStream = AssetLoader.Open(grammarUri);
                if (grammarStream is null) continue;

                using var grammarReader = new StreamReader(grammarStream);
                _availableGrammars.Add(GrammarReader.ReadGrammarSync(grammarReader));
            }


            _availableGrammarDefinitions.Add(grammarDefinition);
        }
    }

    private static string GetThemeFile(ThemeName name) {
        return name switch {
            ThemeName.Abbys => "abyss-color-theme.json",
            ThemeName.Dark => "dark_vs.json",
            ThemeName.DarkPlus => "dark_plus.json",
            ThemeName.DimmedMonokai => "dimmed-monokai-color-theme.json",
            ThemeName.KimbieDark => "kimbie-dark-color-theme.json",
            ThemeName.Light => "light_vs.json",
            ThemeName.LightPlus => "light_plus.json",
            ThemeName.Monokai => "monokai-color-theme.json",
            ThemeName.QuietLight => "quietlight-color-theme.json",
            ThemeName.Red => "Red-color-theme.json",
            ThemeName.SolarizedDark => "solarized-dark-color-theme.json",
            ThemeName.SolarizedLight => "solarized-light-color-theme.json",
            ThemeName.TomorrowNightBlue => "tomorrow-night-blue-color-theme.json",
            ThemeName.HighContrastLight => "hc_light.json",
            ThemeName.HighContrastDark => "hc_black.json",
            _ => throw new ArgumentOutOfRangeException(nameof(name))
        };
    }

    public IRawTheme? LoadTheme(ThemeName name) => GetTheme(GetThemeFile(name));

    public IRawTheme? GetTheme(string scopeName) {
        using var stream = typeof(GrammarDefinition)
            .GetTypeInfo().Assembly
            .GetManifestResourceStream("TextMateSharp.Grammars.Resources.Themes." + scopeName);
        if (stream is null) return null;

        using var reader = new StreamReader(stream);
        return ThemeReader.ReadThemeSync(reader);
    }

    public IRawGrammar? GetGrammar(string scopeName) {
        return _availableGrammars.Find(availableGrammar => string.Equals(availableGrammar.GetScopeName(), scopeName));
    }


    public GrammarDefinition? GetGrammarDefinition(string scopeName) {
        return _availableGrammarDefinitions.Find(availableGrammar => string.Equals(availableGrammar.Name, scopeName));
    }

    public ICollection<string> GetInjections(string scopeName) => [];

    public IRawTheme? GetDefaultTheme() => LoadTheme(_defaultTheme);
}
