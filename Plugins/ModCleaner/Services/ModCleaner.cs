using CreationEditor;
using CreationEditor.Services.DataSource;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Serilog;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.Services;

public sealed class ModCleaner(
    ILogger logger,
    AssetCleaner assetCleaner,
    RecordCleaner recordCleaner) {

    public void Clean(ISkyrimModGetter mod, HashSet<ILinkIdentifier> retained, IDataSource? dataSource) {
        var recordsToClean = RecordCleaner.GetRecordsToClean(retained, mod);

        if (dataSource is not null) {
            var assetsToClean = assetCleaner.GetAssetsToClean(retained, dataSource);

            var m = string.Join("\n", assetsToClean.Select(x => x.DataRelativePath.Path));
            logger.Here().Verbose("Cleaning assets: {Assets}", m);

            assetCleaner.CleanDataSource(dataSource, assetsToClean);
        }

        recordCleaner.CreatedCleanedMod(mod, recordsToClean);
    }

    public Graph<ILinkIdentifier, Edge<ILinkIdentifier>> BuildGraph(IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var graph = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();

        recordCleaner.BuildGraph(graph, mod, dependencies);
        assetCleaner.BuildGraph(graph, mod, dependencies);

        return graph;
    }

    public (HashSet<ILinkIdentifier> AllRetained, Graph<ILinkIdentifier, Edge<ILinkIdentifier>> DependencyGraph) FindRetainedRecords(
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
                    recordCleaner.RetainLinks(graph, mod, dependencies, formLinkIdentifier, retained, excludedLinks, dependencyGraph, RetainOutgoingEdges);
                    break;
                }
                case AssetLinkIdentifier assetLinkIdentifier: {
                    assetCleaner.RetainLinks(graph, mod, dependencies, assetLinkIdentifier, retained, excludedLinks, dependencyGraph, RetainOutgoingEdges);
                    break;
                }
            }
        }

        recordCleaner.FinalRetainLinks(graph, retained, excludedLinks, dependencyGraph, RetainOutgoingEdges);

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
