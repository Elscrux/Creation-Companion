using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References;
using ModCleaner.Models;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Serilog;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.Services;

public sealed class AssetCleaner(
    ILogger logger,
    IAssetTypeProvider assetTypeProvider,
    IAssetTypeService assetTypeService,
    IAssetController assetController,
    IReferenceService referenceService,
    IDataSourceService dataSourceService) {

    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        foreach (var fileLink in dataSourceService.EnumerateFileLinksInAllDataSources(new DataRelativePath(string.Empty), true)) {
            var assetLink = assetTypeService.GetAssetLink(fileLink.DataRelativePath);
            if (assetLink is null) continue;

            var assetLinkIdentifier = new AssetLinkIdentifier(assetLink);
            graph.AddVertex(assetLinkIdentifier);

            foreach (var recordReference in referenceService.GetRecordReferences(assetLink)) {
                if (mod.ModKey == recordReference.FormKey.ModKey || dependencies.Contains(recordReference.FormKey.ModKey)) {
                    graph.AddEdge(new Edge<ILinkIdentifier>(new FormLinkIdentifier(recordReference), assetLinkIdentifier));
                }
            }

            foreach (var nifReference in referenceService.GetAssetReferences(assetLink)) {
                try {
                    var nifLink = new AssetLinkGetter<SkyrimModelAssetType>(nifReference);
                    graph.AddEdge(new Edge<ILinkIdentifier>(new AssetLinkIdentifier(nifLink), assetLinkIdentifier));
                } catch (Exception e) {
                    logger.Here().Error(e, "Error creating asset link for {Asset}", nifReference);
                }
            }
        }
    }

    public IReadOnlyList<IAssetLinkGetter> GetAssetsToClean(HashSet<ILinkIdentifier> included, IDataSource dataSource) {
        return dataSource.EnumerateFiles(new DataRelativePath(string.Empty), includeSubDirectories: true)
            .Select(assetTypeService.GetAssetLink)
            .WhereNotNull()
            .Except(included.OfType<AssetLinkIdentifier>().Select(x => x.AssetLink))
            .ToArray();
    }

    public void CleanDataSource(IDataSource dataSource, IReadOnlyList<IAssetLinkGetter> assetsToClean) {
        foreach (var assetLinkGetter in assetsToClean) {
            try {
                var fileLink = new FileSystemLink(dataSource, assetLinkGetter.DataRelativePath);
                assetController.Delete(fileLink);
            } catch (Exception e) {
                logger.Here().Error(e, "Error deleting asset {Asset}", assetLinkGetter.DataRelativePath);
            }
        }
    }

    private readonly IAssetType[] _selfRetainingAssetTypes = [
        assetTypeProvider.Behavior,
    ];

    public void IncludeLinks(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies,
        AssetLinkIdentifier assetLinkIdentifier,
        HashSet<ILinkIdentifier> included,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        if (_selfRetainingAssetTypes.Contains(assetLinkIdentifier.AssetLink.Type)) {
            // Always retain behavior assets
            included.Add(assetLinkIdentifier);
        }
    }
}
