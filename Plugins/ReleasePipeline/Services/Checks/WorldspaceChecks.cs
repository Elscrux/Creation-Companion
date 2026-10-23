using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

public sealed class WorldspaceChecks : IReleaseCheck, IIsolatedRecordAnalyzer<IWorldspaceGetter> {
    public string Name => "Worldspace Large References Missing";

    public static readonly TopicDefinition WorldspaceLargeReferencesMissing = CheckTopic.Custom("Worldspace Large References Missing", Severity.Error)
        .WithoutFormatting("Worldspace has no large references generated");

    public static readonly TopicDefinition WorldspaceMaxHeightMissing = CheckTopic.Custom("Worldspace MaxHeight Data Missing", Severity.Warning)
        .WithoutFormatting("Worldspace has no max height data generated");

    public IEnumerable<TopicDefinition> Topics { get; } = [
        WorldspaceLargeReferencesMissing,
        WorldspaceMaxHeightMissing,
    ];

    public void AnalyzeRecord(IsolatedRecordAnalyzerParams<IWorldspaceGetter> param) {
        var worldspace = param.Record;

        if (worldspace.LargeReferences is null) {
            param.AddTopic(WorldspaceLargeReferencesMissing.Format());
        }

        if (worldspace.MaxHeight is null) {
            param.AddTopic(WorldspaceMaxHeightMissing.Format());
        }
    }

    public IEnumerable<Func<IWorldspaceGetter, object?>> FieldsOfInterest() {
        yield return x => x.LargeReferences;
        yield return x => x.MaxHeight;
    }
}
