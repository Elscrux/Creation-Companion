namespace ReleasePipeline.Models.Actions;

/// <summary>Result of running a single <see cref="IPipelineAction"/> in a pipeline run.</summary>
public sealed record ActionRunOutcome(IPipelineAction Action, PipelineActionResult Result);

/// <summary>Aggregate result of running a pipeline's ordered actions.</summary>
public sealed record PipelineRunOutcome(bool Succeeded, IReadOnlyList<ActionRunOutcome> ActionOutcomes);
