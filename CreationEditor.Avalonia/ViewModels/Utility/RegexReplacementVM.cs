using System.Text.RegularExpressions;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Utility;

/// <summary>
/// Represents a pair of regex pattern and replacement.
/// </summary>
public sealed partial class RegexReplacementVM : ViewModel {
    [Reactive] public partial string Pattern { get; set; } = string.Empty;
    [Reactive] public partial string Replacement { get; set; } = string.Empty;

    public RegexReplacementVM() {}

    public RegexReplacementVM(string pattern, string replacement) {
        Pattern = pattern;
        Replacement = replacement;
    }

    public string Replace(string name) {
        if (string.IsNullOrWhiteSpace(Pattern)) return name;

        return Regex.Replace(name, Pattern, Replacement);
    }
}
