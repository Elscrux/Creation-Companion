using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

/// <summary>
/// Reports duplicate editor IDs within the mod, which can point at copy mistakes.
/// </summary>
public sealed class DuplicateEditorIDCheck : IReleaseCheck, IContextualAnalyzer {
    public string Name => "Duplicate Editor IDs";

    public static readonly TopicDefinition DuplicateEditorID = CheckTopic.Custom("Duplicate Editor ID", Severity.Warning)
        .WithoutFormatting("Editor ID is used by more than one record");

    public IEnumerable<TopicDefinition> Topics { get; } = [DuplicateEditorID];

    public void Analyze(ContextualAnalyzerParams param) {
        var duplicates = param.LinkCache.PriorityOrder
            .SelectMany(mod => mod.EnumerateMajorRecords())
            .Where(r => !string.IsNullOrEmpty(r.EditorID))
            .DistinctBy(r => r.FormKey)
            .GroupBy(r => r.EditorID!)
            .Where(g => g.Count() > 1)
            .ToArray();

        foreach (var group in duplicates) {
            foreach (var record in group) {
                param.AddTopic(record.FormKey.ModKey, record, DuplicateEditorID.Format());
            }
        }
    }
}
