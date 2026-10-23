using CreationEditor.Services.Config;
namespace ReleasePipeline.Models.Actions;

/// <summary>
/// Non-generic base for a configurable step in a <see cref="Pipeline"/>. Used to store
/// heterogeneous actions in a single list; concrete actions implement.
/// </summary>
public interface IPipelineAction {
    /// <summary>Stable identifier used to persist and rehydrate the action</summary>
    Guid Id { get; }

    /// <summary>Display name shown in the UI</summary>
    string DisplayName { get; }

    /// <summary>Short description shown in the UI</summary>
    string Description { get; }

    /// <summary>
    /// Whether the action modifies files in place.
    /// The runner copies the data source into a temporary working directory before
    /// the first such action runs, so the data source itself is never modified.
    /// </summary>
    bool RequiresInPlaceModification { get; }

    /// <summary>The action's configuration, persisted alongside the action so it round-trips</summary>
    IConfigDefinition ConfigDefinition { get; }

    /// <summary>The action's configuration, persisted alongside the action so it round-trips</summary>
    ConfigValues ConfigValues { get; set; }

    /// <summary>Executes the action within the given pipeline context</summary>
    Task<PipelineActionResult> Execute(PipelineContext context, CancellationToken token);
}
