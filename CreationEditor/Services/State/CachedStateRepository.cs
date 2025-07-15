namespace CreationEditor.Services.State;

public sealed class CachedStateRepository<TStateOut, TState, TIdentifier>(
    IEnumerable<string> stateIds,
    IStateRepositoryFactory<TStateOut, TState, TIdentifier> repoFactory)
    : ICachedStateRepository<TStateOut, TState, TIdentifier>
    where TStateOut : class, TState
    where TState : class
    where TIdentifier : notnull {
    private readonly IStateRepository<TStateOut, TState, TIdentifier> _wrappedRepo = repoFactory.Create(stateIds);
    private readonly Dictionary<TIdentifier, TStateOut> _cache = new();

    public int Count() => _wrappedRepo.Count();
    public IEnumerable<TIdentifier> LoadAllIdentifiers() => _wrappedRepo.LoadAllIdentifiers();
    public IEnumerable<TStateOut> LoadAll() => _wrappedRepo.LoadAll();
    public IReadOnlyDictionary<TIdentifier, TState> LoadAllWithIdentifier() => _wrappedRepo.LoadAllWithIdentifier();
    public TStateOut? Load(TIdentifier id) {
        if (_cache.TryGetValue(id, out var value)) return value;

        value = _wrappedRepo.Load(id);
        if (value is null) return value;

        _cache[id] = value;
        return value;
    }
    public bool Save(TState state, TIdentifier id) {
        if (state is not TStateOut stateT) {
            throw new ArgumentException($"State must be of type {typeof(TStateOut).Name}", nameof(state));
        }

        _cache[id] = stateT;
        return _wrappedRepo.Save(state, id);
    }
    public bool Update(Func<TState?, TState> state, TIdentifier id) => _wrappedRepo.Update(state, id);
    public void Delete(TIdentifier id) {
        _cache.Remove(id);

        _wrappedRepo.Delete(id);
    }
}
