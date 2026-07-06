using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.Services;

public sealed class ModCleaner(
    IEditorEnvironment editorEnvironment,
    AssetCleaner assetCleaner,
    RecordCleaner recordCleaner) {

    /// <summary>
    /// Cleans the given mod by removing records and assets that are not retained.
    /// </summary>
    /// <param name="mod">Mod to clean</param>
    /// <param name="retained">Record and assets links to retain</param>
    /// <param name="dataSource">Data source to clean</param>
    public void Clean(ISkyrimModGetter mod, HashSet<ILinkIdentifier> retained, IDataSource? dataSource) {
        var recordsToClean = RecordCleaner.GetRecordsToClean(retained, mod);

        if (dataSource is not null) {
            var assetsToClean = assetCleaner.GetAssetsToClean(retained, dataSource, mod);

            assetCleaner.CleanDataSource(dataSource, assetsToClean);
        }

        recordCleaner.CreatedCleanedMod(mod, recordsToClean);
    }

    /// <summary>
    /// Builds a graph for the given mod and its dependencies which lists all the links between within a mod and all linked assets.
    /// </summary>
    /// <param name="mod">Mod to build graph for</param>
    /// <param name="dependencies">List of dependencies</param>
    /// <returns>Link graph</returns>
    public Graph<ILinkIdentifier, Edge<ILinkIdentifier>> BuildGraph(IModGetter mod, IReadOnlyList<ModKey> dependencies) {
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
    /// <param name="mod">Mod to find retained records for</param>
    /// <param name="dependencies">List of mods that are dependent on the mod, any links to the mod in the dependencies will be retained</param>
    /// <param name="excludedLinks">Set of links to exclude from retention</param>
    /// <returns>Tuple of retained links and a dependency graph which shows where the retained links were first referenced from for debugging</returns>
    public (HashSet<ILinkIdentifier> AllRetained, Graph<ILinkIdentifier, Edge<ILinkIdentifier>> DependencyGraph) FindRetainedRecords(
        IEssentialRecordProvider essentialRecordProvider,
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies,
        IReadOnlySet<ILinkIdentifier> excludedLinks) {
        var retained = new HashSet<ILinkIdentifier>();
        var dependencyGraph = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();

        foreach (var vertex in graph.Vertices) {
            if (retained.Contains(vertex)) continue;
            if (excludedLinks.Contains(vertex)) continue;

            switch (vertex) {
                case FormLinkIdentifier formLinkIdentifier: {
                    recordCleaner.RetainLinks(essentialRecordProvider, graph, mod, dependencies, formLinkIdentifier, retained, excludedLinks, dependencyGraph, RetainOutgoingEdges);
                    break;
                }
                case AssetLinkIdentifier assetLinkIdentifier: {
                    assetCleaner.RetainLinks(graph, mod, dependencies, assetLinkIdentifier, retained, excludedLinks, dependencyGraph, RetainOutgoingEdges);
                    break;
                }
            }
        }

        recordCleaner.FinalRetainLinks(essentialRecordProvider, graph, retained, excludedLinks, dependencyGraph, RetainOutgoingEdges);

        return (retained, dependencyGraph);

        void RetainOutgoingEdges(HashSet<Edge<ILinkIdentifier>> edges) {
            if (edges.Count == 0) return;

            var queue = new Queue<ILinkIdentifier>(edges.Select(x => x.Target));
            var source = edges.First().Source;
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                if (excludedLinks.Contains(current)) continue;

                if (current != source) {
                    dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(source, current));
                }

                if (!retained.Add(current)) continue;
                if (!graph.OutgoingEdges.TryGetValue(current, out var currentEdges)) continue;

                foreach (var edge in currentEdges) {
                    queue.Enqueue(edge.Target);
                }
            }
        }
    }
}
