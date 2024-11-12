namespace CreationEditor.Services.Filter;

public sealed class WildcardSearchFilter : ISearchFilter {
    private const char SplitChar = '*';

    public bool Filter(string content, string searchTerm) {
        if (string.IsNullOrWhiteSpace(searchTerm)) return true;

        var contentSpan = content.AsSpan();
        var searchTermSpan = searchTerm.AsSpan();

        var contentIndex = 0;
        foreach (var termRange in searchTermSpan.Trim().Split(SplitChar)) {
            var term = searchTermSpan[termRange];
            var indexOfTermInContent = contentSpan[contentIndex..].IndexOf(term, StringComparison.OrdinalIgnoreCase);
            if (indexOfTermInContent == -1) return false;

            contentIndex = indexOfTermInContent;
        }

        return true;
    }
}
