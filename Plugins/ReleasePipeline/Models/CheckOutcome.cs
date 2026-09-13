using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Models;

/// <summary>Result of running a single <see cref="Check.IReleaseCheck"/> against a mod.</summary>
public sealed record CheckOutcome(IReleaseCheck Check, CheckResult Result);
