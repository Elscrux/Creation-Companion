namespace CreationEditor.Services.State;

public class StateRepositoryFactory<TState, TStateIn, TIdentifier>(
    Func<IEnumerable<string>, IStateRepository<TState, TStateIn, TIdentifier>> factory,
    Func<IEnumerable<string>, ICachedStateRepository<TState, TStateIn, TIdentifier>> factoryCached)
    : IStateRepositoryFactory<TState, TStateIn, TIdentifier>
    where TState : class, TStateIn
    where TStateIn : class
    where TIdentifier : notnull {
    public IStateRepository<TState, TStateIn, TIdentifier> Create(string stateId) => factory([stateId]);
    public IStateRepository<TState, TStateIn, TIdentifier> Create(params IEnumerable<string> stateIds) => factory(stateIds);
    public IStateRepository<TState, TStateIn, TIdentifier> CreateCached(string stateId) => factoryCached([stateId]);
    public IStateRepository<TState, TStateIn, TIdentifier> CreateCached(params IEnumerable<string> stateIds) => factoryCached(stateIds);
}
