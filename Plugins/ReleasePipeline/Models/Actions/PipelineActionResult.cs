namespace ReleasePipeline.Models.Actions;

public enum RunStatus {
    Pending,
    Running,
    Passed,
    Failed,
    Warned,
    Skipped,
}

public sealed record PipelineActionResult(RunStatus Status, string Message) {
    public static PipelineActionResult Pass(string message) => new(RunStatus.Passed, message);
    public static PipelineActionResult Fail(string message) => new(RunStatus.Failed, message);
    public static PipelineActionResult Warn(string message) => new(RunStatus.Warned, message);
    public static PipelineActionResult Skipped(string message) => new(RunStatus.Skipped, message);
}
