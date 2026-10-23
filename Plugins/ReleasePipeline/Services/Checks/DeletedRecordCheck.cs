using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins.Records;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

/// <summary>
/// Record check that looks for deleted records in a mod which should never remain in a release build.
/// </summary>
public sealed class DeletedRecordCheck : IReleaseCheck, IIsolatedRecordAnalyzer<IMajorRecordGetter> {
    public string Name => "Deleted Record";

    public static readonly TopicDefinition DeletedRecord = CheckTopic.Custom("Deleted Record", Severity.Error)
        .WithoutFormatting("Record is marked as deleted");

    public IEnumerable<TopicDefinition> Topics { get; } = [DeletedRecord];

    public void AnalyzeRecord(IsolatedRecordAnalyzerParams<IMajorRecordGetter> param) {
        if (param.Record.IsDeleted) {
            param.AddTopic(DeletedRecord.Format());
        }
    }

    public IEnumerable<Func<IMajorRecordGetter, object?>> FieldsOfInterest() {
        yield return x => x.IsDeleted;
    }
}
