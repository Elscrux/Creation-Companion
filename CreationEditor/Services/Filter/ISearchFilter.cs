namespace CreationEditor.Services.Filter;

public interface ISearchFilter {
    /// <summary>
    /// Check if the search word includes the search term
    /// </summary>
    /// <param name="content">word to search in</param>
    /// <param name="searchTerm">term to search by</param>
    /// <returns>true if the search word includes the search term</returns>
    bool Filter(string content, string searchTerm);
}
