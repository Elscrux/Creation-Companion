using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Analyzers.SDK.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

public sealed class MissingAssetsCheck(IDataSourceService dataSourceService) : IReleaseCheck, IContextualAnalyzer {
    public string Name => "Missing Assets";
    public PipelineContext? Context { get; set; }

    public static readonly TopicDefinition<DataRelativePath> MissingAssets = CheckTopic.Custom("Missing Assets", Severity.Warning)
        .WithFormatting<DataRelativePath>("Record is missing asset: {0}");

    public IEnumerable<TopicDefinition> Topics { get; } = [MissingAssets];

    public void Analyze(ContextualAnalyzerParams param) {
        var assetLinkCache = param.LinkCache.CreateImmutableAssetLinkCache();

        foreach (var mod in param.LinkCache.PriorityOrder) {
            if (Context is not null && Context.Mod.ModKey != mod.ModKey) continue;

            foreach (var record in mod.EnumerateMajorRecords()) {
                foreach (var assetLink in record.EnumerateAssetLinks(AssetLinkQuery.Listed | AssetLinkQuery.Inferred, assetLinkCache)) {
                    if (string.IsNullOrEmpty(assetLink.DataRelativePath.Path)) continue;
                    if (dataSourceService.FileExists(assetLink.DataRelativePath)) continue;

                    param.AddTopic(
                        mod.ModKey,
                        record,
                        MissingAssets.Format(assetLink.DataRelativePath));
                }
            }
        }
    }

    public IEnumerable<Func<ICellGetter, object?>> FieldsOfInterest() {
        yield break;
    }
}
