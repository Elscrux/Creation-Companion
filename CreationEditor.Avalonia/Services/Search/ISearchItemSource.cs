using CreationEditor.Services.Search;
using DynamicData;
namespace CreationEditor.Avalonia.Services.Search;

/// <summary>
/// Provides the search items of one or more scopes.
/// Items are updated over time, e.g. when the environment or data sources change.
/// </summary>
public interface ISearchItemSource : IDisposable {
    /// <summary>
    /// Change set of all search items of this source.
    /// </summary>
    IObservable<IChangeSet<SearchItem, string>> Items { get; }
}
