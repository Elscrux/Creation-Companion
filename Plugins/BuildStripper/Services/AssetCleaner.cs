using BuildStripper.Models;
using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Serilog;
using ILinkIdentifier = BuildStripper.Models.ILinkIdentifier;
namespace BuildStripper.Services;

public sealed class AssetCleaner(
    ILogger logger,
    IAssetTypeProvider assetTypeProvider,
    IAssetTypeService assetTypeService,
    IAssetController assetController,
    IReferenceService referenceService,
    IDataSourceService dataSourceService) {

    /// <summary>
    /// Adds all references to assets to the reference graph.
    /// </summary>
    /// <param name="graph">Reference graph to add references to</param>
    /// <param name="mod">Mod to get references for</param>
    /// <param name="dependencies">List of mods that are dependent on the mod in question and are relevant for the reference graph</param>
    /// <param name="masters">List of masters of the mod</param>
    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies, IReadOnlyList<ModKey> masters) {
        // Build graph with all files in all loaded data sources, not just the selected data source
        // Some assets in the selected data source might also be used by files in data sources of mods dependent on the selected mod
        // So we'd expect that the mods using our selected mod as a dependency also
        foreach (var fileLink in dataSourceService.EnumerateFileLinksInAllDataSources(new DataRelativePath(string.Empty), true)) {
            var assetLink = assetTypeService.GetAssetLink(fileLink.DataRelativePath);
            if (assetLink is null) continue;

            var assetLinkIdentifier = new AssetLinkIdentifier(assetLink);
            graph.AddVertex(assetLinkIdentifier);

            foreach (var recordReference in referenceService.GetRecordReferences(assetLink)) {
                if (mod.ModKey == recordReference.FormKey.ModKey
                 || masters.Contains(recordReference.FormKey.ModKey)
                 || dependencies.Contains(recordReference.FormKey.ModKey)) {
                    // Worldspace references everything that any of its nested cells reference, ignore all this
                    if (recordReference.Type == typeof(IWorldspaceGetter)
                     && assetLink.DataRelativePath.Path.StartsWith(@"Textures\Water\", DataRelativePath.PathComparison)) {
                        continue;
                    }

                    graph.AddEdge(new Edge<ILinkIdentifier>(new FormLinkIdentifier(recordReference), assetLinkIdentifier));
                }
            }

            foreach (var nifReference in referenceService.GetAssetReferences(assetLink)) {
                var nifLink = assetTypeService.GetAssetLink(nifReference);
                if (nifLink is null) {
                    logger.Here().Error("Error creating asset link for {Asset}", nifReference);
                    continue;
                }

                graph.AddEdge(new Edge<ILinkIdentifier>(new AssetLinkIdentifier(nifLink), assetLinkIdentifier));
            }
        }
    }

    public IReadOnlyList<IAssetLinkGetter> GetAssetsToClean(HashSet<ILinkIdentifier> retained, IDataSource dataSource, ISkyrimModGetter mod) {
        return dataSource.EnumerateFiles(new DataRelativePath(string.Empty), includeSubDirectories: true)
            .Select(link => link.DataRelativePath)
            .Select(assetTypeService.GetAssetLink)
            .WhereNotNull()
            .Except(retained.OfType<AssetLinkIdentifier>().Select(x => x.AssetLink))
            .Concat(mod.EnumerateInferredAssetLinks<SkyrimTranslationAssetType>())
            .ToArray();
    }

    public void CleanDataSource(IDataSource dataSource, IReadOnlyList<IAssetLinkGetter> assetsToClean) {
        foreach (var assetLinkGetter in assetsToClean) {
            try {
                var fileLink = new DataSourceFileLink(dataSource, assetLinkGetter.DataRelativePath);
                assetController.Delete(fileLink);
            } catch (Exception e) {
                logger.Here().Error(e, "Error deleting asset {Asset}", assetLinkGetter.DataRelativePath);
            }
        }
    }

    private readonly IAssetType[] _selfRetainingAssetTypes = [
        assetTypeProvider.Behavior,
    ];

    public void RetainLinks(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        FilteredGraph<ILinkIdentifier, Edge<ILinkIdentifier>> retainedGraph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies,
        AssetLinkIdentifier assetLinkIdentifier) {
        if (_selfRetainingAssetTypes.Contains(assetLinkIdentifier.AssetLink.Type)) {
            // Always retain behavior assets
            retainedGraph.IncludeVertex(assetLinkIdentifier, assetLinkIdentifier);
        }
    }
}
