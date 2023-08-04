namespace CreationEditor.Services.Filter;

public sealed class WildcardSearchFilter : ISearchFilter {
    private const char SplitChar = '*';

    public bool Filter(string searchWord, string searchTerm) {
        if (string.IsNullOrWhiteSpace(searchTerm)) return true;

        var index = 0;
        foreach (var term in searchTerm.Trim().Split(SplitChar)) {
            var indexOf = searchWord[index..].IndexOf(term, StringComparison.OrdinalIgnoreCase);
            if (indexOf == -1) return false;

            index = indexOf;
        }

        return true;
    }
}
