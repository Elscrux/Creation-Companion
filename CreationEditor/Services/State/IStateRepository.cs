namespace CreationEditor.Services.State;

public interface IStateRepository<TState, TIdentifier>
    where TState : class
    where TIdentifier : notnull {
    int Count();

    IEnumerable<string> GetNeighboringStates();
    IEnumerable<TIdentifier> LoadAllIdentifiers();
    IEnumerable<TState> LoadAll();
    IReadOnlyDictionary<TIdentifier, TState> LoadAllWithIdentifier();
    TState? Load(TIdentifier id);

    bool Save(TState state, TIdentifier id);

    void Delete(TIdentifier id);
}