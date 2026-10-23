using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

/// <summary>
/// Checks worldspace cells for missing occlusion data, which should be generated for all exterior cells for a released build.
/// </summary>
public sealed class MissingOcclusionCheck : IReleaseCheck, IIsolatedRecordAnalyzer<ICellGetter> {
    public string Name => "Missing Cell Occlusion Data";

    public static readonly TopicDefinition MissingOcclusionData = CheckTopic.Custom("Missing Cell Occlusion Data", Severity.Warning)
        .WithoutFormatting("Cell has no occlusion data");

    public IEnumerable<TopicDefinition> Topics { get; } = [MissingOcclusionData];

    public void AnalyzeRecord(IsolatedRecordAnalyzerParams<ICellGetter> param) {
        var cell = param.Record;
        if (cell.Grid is null) return;

        if (cell.OcclusionData is null) {
            param.AddTopic(MissingOcclusionData.Format());
        }
    }

    public IEnumerable<Func<ICellGetter, object?>> FieldsOfInterest() {
        yield return x => x.OcclusionData;
    }
}
