namespace ReleasePipeline.Models.Actions;

/// <summary>
/// Non-generic base for a configurable step in a <see cref="Pipeline"/>. Used to store
/// heterogeneous actions in a single list; concrete actions implement
/// <see cref="IPipelineAction{TConfig}"/> for strongly-typed configuration.
/// </summary>
public interface IPipelineAction {
    /// <summary>Stable identifier used to persist and rehydrate the action.</summary>
    public Guid Id { get; }

    /// <summary>Display name shown in the UI.</summary>
    public string DisplayName { get; }

    /// <summary>Short description shown in the UI.</summary>
    public string Description { get; }

    /// <summary>Concrete type of the action's configuration, used to instantiate and render it.</summary>
    public Type ConfigType { get; }

    public Task<PipelineActionResult> ExecuteAsync(PipelineContext context, CancellationToken token);
}

/// <summary>
/// A configurable step in a <see cref="Pipeline"/> with a strongly-typed, JSON-serialized
/// configuration. A concrete action both holds its configuration and knows how to execute
/// against a <see cref="PipelineContext"/>.
/// </summary>
/// <typeparam name="TConfig">Concrete configuration type for this action.</typeparam>
public interface IPipelineAction<TConfig> : IPipelineAction
    where TConfig : class {
    /// <summary>The action's configuration, persisted alongside the action so it round-trips.</summary>
    public TConfig Config { get; set; }
}
