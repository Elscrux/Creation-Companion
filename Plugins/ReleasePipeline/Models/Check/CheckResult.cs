namespace ReleasePipeline.Models.Check;

public enum CheckSeverity {
    /// <summary>Blocking problem — the mod should not be released until resolved.</summary>
    Error,
    /// <summary>Non-blocking concern worth inspecting before release.</summary>
    Warning,
}

public enum CheckStatus {
    Pending,
    Running,
    Passed,
    Failed,
    Warned,
}

public sealed class CheckResult {
    public CheckStatus Status { get; }
    public string? Message { get; }

    public CheckResult(CheckStatus status, string? message = null) {
        Status = status;
        Message = message;
    }

    public static CheckResult Pass() => new(CheckStatus.Passed);
    public static CheckResult Fail(string message) => new(CheckStatus.Failed, message);
    public static CheckResult Warn(string message) => new(CheckStatus.Warned, message);
}
