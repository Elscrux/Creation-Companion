using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins.Records;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

/// <summary>
/// Flags records whose editor ID looks unfinished, deletable, or otherwise suspicious.
/// </summary>
public sealed class UnfinishedEditorIDCheck : IReleaseCheck, IIsolatedRecordAnalyzer<IMajorRecordGetter> {
    private static readonly string[] DisallowedFragments = ["delete", "remove"];
    private static readonly string[] DisallowedStarts = ["x"];

    public string Name => "Unfinished Editor IDs";

    public static readonly TopicDefinition SuspiciousEditorID = CheckTopic.Custom("Unfinished Editor ID", Severity.Warning)
        .WithoutFormatting("Record has a suspicious editor ID");

    public IEnumerable<TopicDefinition> Topics { get; } = [SuspiciousEditorID];

    public void AnalyzeRecord(IsolatedRecordAnalyzerParams<IMajorRecordGetter> param) {
        var editorId = param.Record.EditorID;
        if (string.IsNullOrEmpty(editorId)) return;

        if (DisallowedStarts.Any(s => editorId.StartsWith(s, StringComparison.OrdinalIgnoreCase)) ||
            DisallowedFragments.Any(f => editorId.Contains(f, StringComparison.OrdinalIgnoreCase))) {
            param.AddTopic(SuspiciousEditorID.Format());
        }
    }

    public IEnumerable<Func<IMajorRecordGetter, object?>> FieldsOfInterest() {
        yield return x => x.EditorID;
    }
}
