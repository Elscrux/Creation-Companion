using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Models;

/// <summary>
/// Top-level, globally-persisted release configuration. Its <see cref="Name"/> is the target
/// platform (for example a pipeline named "PC"). Holds an ordered list of actions.
/// </summary>
public sealed class Pipeline {
    /// <summary>Pipeline name, which doubles as the platform identifier.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional target folder the release is written into.</summary>
    public string? ReleaseFolder { get; set; }

    /// <summary>Ordered configured actions, executed one by one on release.</summary>
    public List<IPipelineAction> Actions { get; set; } = [];

    public override string ToString() => Name;
}
