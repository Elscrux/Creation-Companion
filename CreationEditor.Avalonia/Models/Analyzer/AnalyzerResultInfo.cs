using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Analyzer;

/// <summary>
///     Enriched analyzer result with resolved display information.
/// </summary>
public sealed class AnalyzerResultInfo {
    public required AnalyzerResult Result { get; init; }
    public string? ResultEditorId { get; init; }
    public string? RecordDisplayName { get; init; }
    public string? ParentDisplayName { get; init; }
    public IMajorRecordIdentifierGetter? ParentIdentifier { get; init; }

    /// <summary>
    ///     Gets a unique identifier for this result instance.
    ///     Used for tracking new/resolved results and ignore rules.
    /// </summary>
    public string GetIdentifier() => $"{Result.Topic.Severity}:{Result.Topic?.TopicDefinition.Id}:{Result.Record?.FormKey}:{Result.ModKey}";
}
