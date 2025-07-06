namespace CreationEditor.Services.State;

public interface IStateRepositoryFactory<out TState, TStateIn, TIdentifier>
    where TState : class, TStateIn
    where TStateIn : class
    where TIdentifier : notnull {
    IStateRepository<TState, TStateIn, TIdentifier> Create(string stateId);
    IStateRepository<TState, TStateIn, TIdentifier> Create(params IEnumerable<string> stateIds);
    IStateRepository<TState, TStateIn, TIdentifier> CreateCached(string stateId);
    IStateRepository<TState, TStateIn, TIdentifier> CreateCached(params IEnumerable<string> stateIds);
}
