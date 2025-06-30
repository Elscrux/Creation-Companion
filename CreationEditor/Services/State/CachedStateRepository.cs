namespace CreationEditor.Services.State;

public sealed class CachedStateRepository<TState, TIdentifier>(IStateRepository<TState, TIdentifier> wrappedRepo)
    : ICachedStateRepository<TState, TIdentifier>
    where TState : class
    where TIdentifier : notnull {
    private readonly Dictionary<TIdentifier, TState> _cache = new();
    
    public int Count() => wrappedRepo.Count();
    public IEnumerable<string> GetNeighboringStates() => wrappedRepo.GetNeighboringStates();
    public IEnumerable<TIdentifier> LoadAllIdentifiers() => wrappedRepo.LoadAllIdentifiers();
    public IEnumerable<TState> LoadAll() => wrappedRepo.LoadAll();
    public IReadOnlyDictionary<TIdentifier, TState> LoadAllWithIdentifier() => wrappedRepo.LoadAllWithIdentifier();
    public TState? Load(TIdentifier id) {
        if (_cache.TryGetValue(id, out var value)) return value;

        value = wrappedRepo.Load(id);
        if (value is null) return value;

        _cache[id] = value;
        return value;
    }
    public bool Save(TState state, TIdentifier id) {
        _cache[id] = state;

        return wrappedRepo.Save(state, id);
    }
    public void Delete(TIdentifier id) {
        _cache.Remove(id);

        wrappedRepo.Delete(id);
    }
}
