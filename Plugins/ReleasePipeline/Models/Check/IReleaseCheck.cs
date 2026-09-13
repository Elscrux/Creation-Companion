using Mutagen.Bethesda.Skyrim;
namespace ReleasePipeline.Models.Check;

/// <summary>
/// A single pre-release check. Checks form a fixed, non-configurable suite that runs before
/// a pipeline's actions. Each check reports pass/fail/warn.
/// </summary>
public interface IReleaseCheck {
    /// <summary>Display name shown in the checks list.</summary>
    string Name { get; }

    /// <summary>See <see cref="CheckSeverity"/>.</summary>
    CheckSeverity Severity { get; }

    /// <summary>Whether this check applies to the given mod. Lets checks opt out (e.g. Beyond Skyrim-specific rules).</summary>
    bool AppliesTo(ISkyrimModGetter mod) => true;

    /// <summary>Runs the check against the mod.</summary>
    Task<CheckResult> RunAsync(ISkyrimModGetter mod, CancellationToken token);
}
