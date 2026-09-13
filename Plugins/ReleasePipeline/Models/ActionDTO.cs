namespace ReleasePipeline.Models;

/// <summary>
/// Persistence DTO for a single pipeline action. Only the action id and its configuration are
/// persisted; display metadata (name, description) is derived from the action type at runtime.
/// </summary>
public sealed class ActionDTO {
    public Guid Id { get; set; }
    public object? Config { get; set; }
}
