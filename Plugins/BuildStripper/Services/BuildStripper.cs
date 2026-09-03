using BuildStripper.Models;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ILinkIdentifier = BuildStripper.Models.ILinkIdentifier;
namespace BuildStripper.Services;

public sealed class BuildStripper(
    IEditorEnvironment editorEnvironment,
    AssetCleaner assetCleaner,
    RecordCleaner recordCleaner) {

    /// <summary>
    /// Cleans the given mod by removing records and assets that are not retained.
    /// </summary>
    /// <param name="mod">Mod to clean</param>
    /// <param name="retained">Record and assets links to retain</param>
    /// <param name="dataSource">Data source to clean</param>
    /// <param name="postProcessSteps">Post process steps to run on retained records after cleaning</param>
    public void Clean(
        ISkyrimModGetter mod,
        HashSet<ILinkIdentifier> retained,
        IDataSource? dataSource,
        IReadOnlyDictionary<IFormLinkIdentifier, Action<IMajorRecord>> postProcessSteps) {
        var recordsToClean = RecordCleaner.GetRecordsToClean(retained, mod);

        if (dataSource is not null) {
            var assetsToClean = assetCleaner.GetAssetsToClean(retained, dataSource, mod);

            assetCleaner.CleanDataSource(dataSource, assetsToClean);
        }

        recordCleaner.CreatedCleanedMod(mod, recordsToClean, postProcessSteps);
    }

    /// <summary>
    /// Builds a graph for the given mod and its dependencies which lists all the links between within a mod and all linked assets.
    /// </summary>
    /// <param name="selectedDataSource">The data source to use for the graph</param>
    /// <param name="mod">Mod to build graph for</param>
    /// <param name="dependencies">List of dependencies</param>
    /// <returns>Link graph</returns>
    public Graph<ILinkIdentifier, Edge<ILinkIdentifier>> BuildGraph(IDataSource? selectedDataSource, IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var graph = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();
        var masters = mod.GetTransitiveMasters(editorEnvironment.GameEnvironment).ToArray();

        recordCleaner.BuildGraph(graph, mod, dependencies, masters);
        assetCleaner.BuildGraph(graph, mod, dependencies, masters);

        return graph;
    }

    /// <summary>
    /// Finds all records and assets that are retained in the given mod and its dependencies, based on a few essential starting records.
    /// </summary>
    /// <param name="essentialRecordProvider">Essential record provider</param>
    /// <param name="graph">Graph of all links in the mod and its dependencies</param>
    /// <param name="selectedDataSource">The data source to use for the graph</param>
    /// <param name="mod">Mod to find retained records for</param>
    /// <param name="dependencies">List of mods that are dependent on the mod, any links to the mod in the dependencies will be retained</param>
    /// <param name="excludedLinks">Set of links to exclude from retention</param>
    /// <returns>Tuple of retained links and a dependency graph which shows where the retained links were first referenced from for debugging</returns>
    public (FilteredGraph<ILinkIdentifier, Edge<ILinkIdentifier>> DependencyGraph, IReadOnlyDictionary<IFormLinkIdentifier, Action<IMajorRecord>> PostProcessSteps)
        FindRetainedRecords(
            IEssentialRecordProvider essentialRecordProvider,
            Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
            IDataSource? selectedDataSource,
            IModGetter mod,
            IReadOnlyList<ModKey> dependencies,
            IReadOnlySet<ILinkIdentifier> excludedLinks) {
        var retainedGraph = new FilteredGraph<ILinkIdentifier, Edge<ILinkIdentifier>>(graph);
        foreach (var excludedLink in excludedLinks) {
            retainedGraph.ExcludeVertex(excludedLink);
        }
        var postProcessSteps = new Dictionary<IFormLinkIdentifier, Action<IMajorRecord>>();

        foreach (var vertex in graph.Vertices) {
            // Skip if excluded
            if (retainedGraph.ExcludedVertices.Contains(vertex)) continue;

            // Skip if already added explicitly
            if (retainedGraph.IncludedVertices.Contains(vertex)) continue;

            switch (vertex) {
                case FormLinkIdentifier formLinkIdentifier: {
                    recordCleaner.RetainLinks(
                        essentialRecordProvider,
                        retainedGraph,
                        mod,
                        dependencies,
                        formLinkIdentifier);
                    break;
                }
                case AssetLinkIdentifier assetLinkIdentifier: {
                    assetCleaner.RetainLinks(graph, retainedGraph, selectedDataSource, mod, dependencies, assetLinkIdentifier);
                    break;
                }
            }
        }

        recordCleaner.FinalRetainLinks(mod, essentialRecordProvider, graph, retainedGraph, AddPostProcessStep);

        return (retainedGraph, postProcessSteps);

        void AddPostProcessStep(IFormLinkIdentifier formLinkIdentifier, Action<IMajorRecord> action) {
            postProcessSteps.TryAdd(formLinkIdentifier, action);
        }
    }
}
