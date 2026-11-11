using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
namespace ReleasePipeline.Models.Check;

/// <summary>
/// A single pre-release check. Checks form a fixed, non-configurable suite that runs before
/// a pipeline's actions. Each check reports pass/fail/warn.
/// </summary>
public interface IReleaseCheck : IAnalyzer {
    /// <summary>Display name shown in the checks list</summary>
    string Name { get; }
    PipelineContext? Context { get; set; }

    /// <summary>Whether this check applies to the given mod. Lets checks opt out (e.g. Beyond Skyrim-specific rules)</summary>
    bool AppliesTo(PipelineContext context) => true;
}
