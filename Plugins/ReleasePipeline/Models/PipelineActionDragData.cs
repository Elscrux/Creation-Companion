using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Models;

/// <summary>Drag payload for reordering pipeline actions.</summary>
public sealed record PipelineActionDragData {
    public required PipelineActionVM Action { get; init; }
}
