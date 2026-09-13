using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Models;

/// <summary>
/// Live, per-action update emitted while a pipeline runs, used for progress reporting.
/// </summary>
public sealed record ActionRunProgress(IPipelineAction Action, RunStatus Status, string? Message = null);
