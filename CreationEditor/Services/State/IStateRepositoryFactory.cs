namespace CreationEditor.Services.State;

public interface IStateRepositoryFactory<TState, TIdentifier>
    where TState : class
    where TIdentifier : notnull {
    IStateRepository<TState, TIdentifier> Create(string stateId);
    IStateRepository<TState, TIdentifier> Create(params IEnumerable<string> stateIds);
}
