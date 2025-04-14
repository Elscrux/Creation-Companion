using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using ModCleaner.Models;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim.Assets;
using Serilog;
namespace ModCleaner.Services;

public sealed class AssetCleaner(
    ILogger logger,
    IAssetTypeService assetTypeService,
    IAssetReferenceController assetReferenceController,
    IDataSourceService dataSourceService) {

    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        foreach (var fileLink in dataSourceService.EnumerateFileLinksInAllDataSources(new DataRelativePath(string.Empty), true)) {
            var assetLink = assetTypeService.GetAssetLink(fileLink.DataRelativePath);
            if (assetLink is null) continue;

            var assetLinkIdentifier = new AssetLinkIdentifier(assetLink);
            graph.AddVertex(assetLinkIdentifier);

            foreach (var recordReference in assetReferenceController.GetRecordReferences(assetLink)) {
                if (mod.ModKey == recordReference.FormKey.ModKey || dependencies.Contains(recordReference.FormKey.ModKey)) {
                    graph.AddEdge(new Edge<ILinkIdentifier>(new FormLinkIdentifier(recordReference), assetLinkIdentifier));
                }
            }

            foreach (var nifReference in assetReferenceController.GetAssetReferences(assetLink)) {
                try {
                    var nifLink = new AssetLinkGetter<SkyrimModelAssetType>(nifReference);
                    graph.AddEdge(new Edge<ILinkIdentifier>(new AssetLinkIdentifier(nifLink), assetLinkIdentifier));
                } catch (Exception e) {
                    logger.Here().Error(e, "Error creating asset link for {Asset}", nifReference);
                }
            }
        }
    }
}
