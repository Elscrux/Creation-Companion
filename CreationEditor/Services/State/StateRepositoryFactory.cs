namespace CreationEditor.Services.State;

public class StateRepositoryFactory<TState, TIdentifier>(Func<IEnumerable<string>, IStateRepository<TState, TIdentifier>> factory)
    : IStateRepositoryFactory<TState, TIdentifier> where TState : class where TIdentifier : notnull {
    public IStateRepository<TState, TIdentifier> Create(string stateId) => factory([stateId]);
    public IStateRepository<TState, TIdentifier> Create(params IEnumerable<string> stateIds) => factory(stateIds);
}
