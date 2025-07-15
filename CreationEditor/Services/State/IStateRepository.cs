namespace CreationEditor.Services.State;

public interface IStateRepository<out TStateOut, TState, TIdentifier>
    where TStateOut : class, TState
    where TState : class
    where TIdentifier : notnull {
    int Count();

    IEnumerable<TIdentifier> LoadAllIdentifiers();
    IEnumerable<TStateOut> LoadAll();
    IReadOnlyDictionary<TIdentifier, TState> LoadAllWithIdentifier();
    TStateOut? Load(TIdentifier id);

    bool Save(TState state, TIdentifier id);

    bool Update(Func<TState?, TState> state, TIdentifier id);

    void Delete(TIdentifier id);
}
