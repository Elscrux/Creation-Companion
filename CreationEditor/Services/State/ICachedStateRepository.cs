namespace CreationEditor.Services.State;

public interface ICachedStateRepository<out TStateOut, TState, TIdentifier> : IStateRepository<TStateOut, TState, TIdentifier>
    where TStateOut : class, TState
    where TState : class
    where TIdentifier : notnull;
