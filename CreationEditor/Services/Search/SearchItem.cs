namespace CreationEditor.Services.Search;

/// <summary>
/// A single item that can appear in the search results.
/// </summary>
/// <param name="Id">Unique, stable identifier of the item. Used for de-duplication and recents.</param>
/// <param name="Title">Main display text that is matched against the search query.</param>
/// <param name="Scope">Scope the item is grouped under in the results.</param>
/// <param name="Subtitle">Optional secondary display text, e.g. FormKey, file path or reason hint.</param>
/// <param name="Icon">Optional icon. Supports FASymbol / FAFontIcon glyph / strings, depending on the UI layer.</param>
/// <param name="Priority">Sort weight within equal match scores. Higher is more prominent.</param>
/// <param name="Keywords">Optional extra searchable terms that don't show up in the UI.</param>
/// <param name="Execute">Action executed when the item is selected. Null items are not selectable.</param>
/// <param name="CanExecute">Optional observable that reflects whether the item can currently be executed.</param>
/// <param name="ContextFactory">Optional factory for the context of the item, used to build context actions.</param>
public sealed record SearchItem(
    string Id,
    string Title,
    SearchScope Scope,
    string? Subtitle = null,
    object? Icon = null,
    int Priority = 0,
    IReadOnlyList<string>? Keywords = null,
    Func<Task>? Execute = null,
    IObservable<bool>? CanExecute = null,
    Func<object?>? ContextFactory = null) {

    /// <summary>
    /// All searchable text of the item, used by the ranking.
    /// </summary>
    public IEnumerable<string> GetSearchableContent() {
        yield return Title;

        if (Subtitle is not null) yield return Subtitle;

        if (Keywords is not null) {
            foreach (var keyword in Keywords) {
                yield return keyword;
            }
        }
    }
}
