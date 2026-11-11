using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

public sealed class RecordOverridesCheck : IReleaseCheck, IContextualAnalyzer {
    public string Name => "Record Overrides";
    public PipelineContext? Context { get; set; }

    public static readonly TopicDefinition<ModKey> RecordOverrides = CheckTopic.Custom("Record Override", Severity.Warning)
        .WithFormatting<ModKey>("Record overrides another record in a different mod: {0}");

    public IEnumerable<TopicDefinition> Topics { get; } = [RecordOverrides];

    public void Analyze(ContextualAnalyzerParams param) {
        foreach (var mod in param.LinkCache.PriorityOrder) {
            if (Context is not null && Context.Mod.ModKey != mod.ModKey) continue;

            foreach (var record in mod.EnumerateMajorRecords()) {
                if (record.FormKey.ModKey == mod.ModKey) continue;

                param.AddTopic(
                    mod.ModKey,
                    record,
                    RecordOverrides.Format(record.FormKey.ModKey));
            }
        }
    }

    public IEnumerable<Func<ICellGetter, object?>> FieldsOfInterest() {
        yield break;
    }
}
