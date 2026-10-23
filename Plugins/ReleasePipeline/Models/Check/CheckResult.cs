using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
namespace ReleasePipeline.Models.Check;

public enum CheckStatus {
    Pending,
    Running,
    Passed,
    Failed,
    Warned,
}

public sealed record CheckResult(CheckStatus Status, List<AnalyzerResult> Results);
