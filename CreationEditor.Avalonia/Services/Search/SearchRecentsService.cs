using CreationEditor.Services.State;
namespace CreationEditor.Avalonia.Services.Search;

/// <summary>
/// Persisted state of recently executed search items.
/// </summary>
public sealed class RecentSearches {
    public List<string> Ids { get; set; } = [];
}

public interface ISearchRecentsService {
    /// <summary>
    /// Get the ids of the recently executed search items, most recent first.
    /// </summary>
    /// <returns>Ids of recent items</returns>
    IReadOnlyList<string> GetRecentIds();

    /// <summary>
    /// Add an item to the recents, moving it to the front if already present.
    /// </summary>
    /// <param name="id">Id of the item</param>
    void Add(string id);

    /// <summary>
    /// Clear all recents.
    /// </summary>
    void Clear();
}

public sealed class SearchRecentsService(IStateRepositoryFactory<RecentSearches, RecentSearches, string> repositoryFactory) : ISearchRecentsService {
    private const string StateId = "SearchRecents";
    private const int MaxRecents = 10;

    private readonly IStateRepository<RecentSearches, RecentSearches, string> _repository = repositoryFactory.CreateCached(StateId);

    public IReadOnlyList<string> GetRecentIds() => _repository.Load(StateId)?.Ids ?? [];

    public void Add(string id) {
        _repository.Update(old => {
            var ids = (old?.Ids ?? [])
                .Where(x => x != id)
                .ToList();
            ids.Insert(0, id);
            if (ids.Count > MaxRecents) ids = ids.Take(MaxRecents).ToList();

            return new RecentSearches { Ids = ids };
        }, StateId);
    }

    public void Clear() {
        _repository.Delete(StateId);
    }
}
