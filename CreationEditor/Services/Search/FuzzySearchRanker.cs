using System.Text;
namespace CreationEditor.Services.Search;

/// <summary>
/// Tiered fuzzy search ranker.
/// Tiers: Exact, Prefix, WordStart, Acronym, Substring, Subsequence.
/// All whitespace-separated query terms must match the content; scores are summed per term.
/// Subsequence matches must start at a word start, so queries without any real anchor
/// in the content do not produce results.
/// </summary>
public sealed class FuzzySearchRanker : ISearchRanker {
    private const double ExactScore = 1000;
    private const double PrefixScore = 800;
    private const double WordStartScore = 600;
    private const double AcronymScore = 450;
    private const double SubstringScore = 400;
    private const double SubsequenceScore = 150;

    private static readonly char[] Separators = [' ', '_', '-', '.', '/', '\\', '(', ')', '[', ']'];

    public SearchMatch? Match(string content, string query, double weight = 1.0) {
        if (string.IsNullOrWhiteSpace(query) || string.IsNullOrEmpty(content)) return null;

        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (terms.Length == 0) return null;

        double totalScore = 0;
        SearchMatchKind? bestKind = null;
        foreach (var term in terms) {
            var match = MatchTerm(content, term);
            if (match is null) return null;

            totalScore += match.Score * weight;
            if (bestKind is null || match.Kind < bestKind) bestKind = match.Kind;
        }

        return new SearchMatch(bestKind!.Value, totalScore);
    }

    private static SearchMatch? MatchTerm(string content, string term) {
        if (content.Equals(term, StringComparison.OrdinalIgnoreCase)) return new SearchMatch(SearchMatchKind.Exact, ExactScore);
        if (content.StartsWith(term, StringComparison.OrdinalIgnoreCase)) return new SearchMatch(SearchMatchKind.Prefix, PrefixScore);

        var index = content.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (index >= 0) {
            var positionBonus = GetPositionBonus(content, index);
            return IsWordStart(content, index)
                ? new SearchMatch(SearchMatchKind.WordStart, WordStartScore + positionBonus)
                : new SearchMatch(SearchMatchKind.Substring, SubstringScore + positionBonus);
        }

        if (IsAcronymMatch(content, term)) return new SearchMatch(SearchMatchKind.Acronym, AcronymScore);

        var subsequenceBonus = GetSubsequenceBonus(content, term);
        if (subsequenceBonus >= 0) return new SearchMatch(SearchMatchKind.Subsequence, SubsequenceScore + subsequenceBonus);

        return null;
    }

    private static double GetPositionBonus(string content, int index) {
        var length = Math.Max(content.Length, 1);
        return (1.0 - index / (double) length) * 100.0;
    }

    private static bool IsWordStart(string content, int index) {
        if (index == 0) return true;

        var previous = content[index - 1];
        if (!char.IsLetterOrDigit(previous)) return true;

        return char.IsUpper(content[index]) && char.IsLower(previous);
    }

    private static bool IsAcronymMatch(string content, string term) {
        var initials = GetWordInitials(content);
        return initials.Contains(term, StringComparison.OrdinalIgnoreCase)
         && initials.Length >= term.Length;
    }

    private static string GetWordInitials(string content) {
        var builder = new StringBuilder();
        foreach (var (c, isStart) in IterateWordStarts(content)) {
            if (isStart) builder.Append(char.ToUpperInvariant(c));
        }

        return builder.ToString();
    }

    private static double GetSubsequenceBonus(string content, string term) {
        var contentIndex = 0;
        double bonus = 0;
        for (var i = 0; i < term.Length; i++) {
            // Find the next occurrence of the query character in the content, starting from contentIndex
            var found = content.IndexOf(term[i].ToString(), contentIndex, StringComparison.OrdinalIgnoreCase);
            if (found == -1) return -1;

            var isWordStart = IsWordStart(content, found);

            // Anchor the match: the first character must line up with a word start,
            // otherwise the query has no real connection to the content
            if (i == 0 && !isWordStart) return -1;

            if (isWordStart) bonus += 20;
            contentIndex = found + 1;
        }

        return bonus;
    }

    private static IEnumerable<(char Char, bool IsWordStart)> IterateWordStarts(string content) {
        return content.Select((t, i) => (t, IsWordStart(content, i) && !Separators.Contains(t)));
    }
}
