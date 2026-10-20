namespace CreationEditor.Services.Search;

public enum SearchScope {
    Action,
    Record,
    Asset,
}

public static class SearchScopeExtensions {
    extension(SearchScope scope) {
        /// <summary>
        /// Display order of the scopes in the search results, from top to bottom.
        /// </summary>
        public int DisplayOrder() => scope switch {
            SearchScope.Record => 0,
            SearchScope.Asset => 1,
            SearchScope.Action => 2,
            _ => int.MaxValue,
        };
        /// <summary>
        /// Header text of the scope group in the search results.
        /// </summary>
        public string GetDisplayName() => scope switch {
            SearchScope.Record => "Records",
            SearchScope.Asset => "Assets",
            SearchScope.Action => "Actions",
            _ => scope.ToString(),
        };
    }
}
