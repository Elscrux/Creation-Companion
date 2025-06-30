namespace CreationEditor.Services.State;

public interface ICachedStateRepository<TState, TIdentifier> : IStateRepository<TState, TIdentifier>
    where TState : class
    where TIdentifier : notnull;
