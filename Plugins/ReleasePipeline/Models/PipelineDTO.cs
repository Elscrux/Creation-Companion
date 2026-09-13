namespace ReleasePipeline.Models;

/// <summary>
/// Persistence DTO for a <see cref="Pipeline"/>. Only the fields that should round-trip to disk
/// are included: the name and the ordered actions. Transient fields such as the release folder
/// are intentionally omitted.
/// </summary>
public sealed class PipelineDTO {
    public string Name { get; set; } = string.Empty;
    public List<ActionDTO> Actions { get; set; } = [];
}
