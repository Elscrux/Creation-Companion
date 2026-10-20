namespace CreationEditor.Services.Search;

public enum SearchMatchKind {
    /// <summary>Query and content are equal (ignoring case).</summary>
    Exact,
    /// <summary>Content starts with the query.</summary>
    Prefix,
    /// <summary>Query matches at the start of a word in the content, e.g. "bo" in "Iron Book".</summary>
    WordStart,
    /// <summary>Query is contained in the content.</summary>
    Substring,
    /// <summary>All query characters appear in order, but not necessarily adjacent, e.g. "ibk" in "IronBook".</summary>
    Subsequence,
    /// <summary>Query is a CamelCase acronym of the content, e.g. "NPC" in "NonPlayerCharacter".</summary>
    Acronym,
}

/// <summary>
/// Ranks content strings against a search query.
/// </summary>
public interface ISearchRanker {
    /// <summary>
    /// Match the given content against the query.
    /// </summary>
    /// <param name="content">Text to match against</param>
    /// <param name="query">Search query</param>
    /// <param name="weight">Optional weight of the content, e.g. title vs keyword</param>
    /// <returns>The match, or null if the content doesn't match the query at all</returns>
    SearchMatch? Match(string content, string query, double weight = 1.0);
}

public sealed record SearchMatch(SearchMatchKind Kind, double Score);
