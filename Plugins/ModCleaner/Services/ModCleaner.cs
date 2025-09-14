using CreationEditor.Services.DataSource;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Serilog;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.Services;

public static class AllPathsFinder
{
    public static List<(TVertex, int)>? CalculateShortestPath<TVertex>(
        this Graph<TVertex, Edge<TVertex>> graph,
        TVertex source,
        TVertex target)
        where TVertex : class {
        // Initialize all the distances to max, and the "previous" city to null
        var distances = graph.Vertices
            .Select(node => (Node: node, Details: (Previous: (TVertex?) null, Distance: int.MaxValue)))
            .ToDictionary(x => x.Node, x => x.Details);

        // priority queue for tracking the shortest distance from the start node to each other node
        var queue = new PriorityQueue<TVertex, int>();

        // initialize the start node at a distance of 0
        distances[source] = (null, 0);

        // add the start node to the queue for processing
        queue.Enqueue(source, 0);

        // as long as we have a node to process, keep looping
        while (queue.Count > 0) {
            // remove the node with the current smallest distance from the start node
            var current = queue.Dequeue();

            // if this is the node we want, then we're finished
            // as we must already have the shortest route!
            if (current == target) {
                // build the route by tracking back through previous
                return BuildRoute(distances, target);
            }

            // add the node to the "visited" list
            var currentNodeDistance = distances[current].Distance;

            if (graph.OutgoingEdges.TryGetValue(current, out var edges)) {

                foreach (var edge in edges) {
                    // get the current shortest distance to the connected node
                    int distance = distances[edge.Target].Distance;
                    // calculate the new cumulative distance to the edge
                    int newDistance = currentNodeDistance + 1;

                    // if the new distance is shorter, then it represents a new 
                    // shortest-path to the connected edge
                    if (newDistance < distance) {
                        // update the shortest distance to the connection
                        // and record the "current" node as the shortest
                        // route to get there 
                        distances[edge.Target] = (current, newDistance);

                        // if the node is already in the queue, first remove it
                        queue.Remove(edge.Target, out _, out _);
                        // now add the node with the new distance
                        queue.Enqueue(edge.Target, newDistance);
                    }
                }
            }
        }

        // if we don't have anything left, then we've processed everything,
        // but didn't find the node we want
        return null;

        static List<(T, int)> BuildRoute<T>(
            Dictionary<T, (T? previous, int Distance)> distances,
            T endNode) where T : class {
            var route = new List<(T, int)>();
            T? prev = endNode;

            // Keep examining the previous version until we
            // get back to the start node
            while (prev is not null) {
                var current = prev;
                (prev, var distance) = distances[current];
                route.Add((current, distance));
            }

            // reverse the route
            route.Reverse();
            return route;
        }
    }

    public static IList<IList<TVertex>> AllPathsBetweenVertices<TVertex, TEdge>(
        this Graph<TVertex, TEdge> graph,
        TVertex source,
        TVertex target)
        where TVertex : class
        where TEdge : Edge<TVertex>
    {
        var result = new List<IList<TVertex>>();
        var path = new List<TVertex>();
        var visited = new HashSet<TVertex>();

        Dfs(source);
        return result;

        void Dfs(TVertex current)
        {
            path.Add(current);
            visited.Add(current);

            if (EqualityComparer<TVertex>.Default.Equals(current, target))
            {
                result.Add(new List<TVertex>(path));
            }
            else if (graph.OutgoingEdges.TryGetValue(current, out var edges))
            {
                foreach (var edge in edges)
                {
                    if (!visited.Contains(edge.Target))
                    {
                        Dfs(edge.Target);
                    }
                }
            }

            path.RemoveAt(path.Count - 1);
            visited.Remove(current);
        }
    }

}
public sealed class ModCleaner(
    ILogger logger,
    AssetCleaner assetCleaner,
    RecordCleaner recordCleaner) {

    public void Clean(ISkyrimModGetter mod, HashSet<ILinkIdentifier> included, IDataSource dataSource) {
        var recordsToClean = recordCleaner.GetRecordsToClean(included, mod);
        // var assetsToClean = assetCleaner.GetAssetsToClean(included, dataSource);
        //
        // var m = string.Join("\n", assetsToClean.Select(x => x.DataRelativePath.Path));
        // logger.Here().Information("Cleaning assets: {Assets}", m);

        // assetCleaner.CleanDataSource(dataSource, assetsToClean);
        recordCleaner.CreatedCleanedMod(mod, recordsToClean);
        Console.WriteLine();
    }

    public Graph<ILinkIdentifier, Edge<ILinkIdentifier>> BuildGraph(IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var graph = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();

        recordCleaner.BuildGraph(graph, mod, dependencies);
        // assetCleaner.BuildGraph(graph, mod, dependencies);

        return graph;
    }

    public (HashSet<ILinkIdentifier> AllIncluded, Graph<ILinkIdentifier, Edge<ILinkIdentifier>> DependencyGraph) FindRetainedRecords(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies) {
        var included = new HashSet<ILinkIdentifier>();
        var dependencyGraph = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();

        foreach (var vertex in graph.Vertices) {
            if (included.Contains(vertex)) continue;

            switch (vertex) {
                case FormLinkIdentifier formLinkIdentifier: {
                    recordCleaner.IncludeLinks(graph, mod, dependencies, formLinkIdentifier, included, dependencyGraph, RetainOutgoingEdges);
                    break;
                }
                case AssetLinkIdentifier assetLinkIdentifier: {
                    assetCleaner.IncludeLinks(graph, mod, dependencies, assetLinkIdentifier, included, dependencyGraph, RetainOutgoingEdges);
                    break;
                }
            }
        }

        recordCleaner.FinalIncludeLinks(graph, included, dependencyGraph, RetainOutgoingEdges);

        return (included, dependencyGraph);

        void RetainOutgoingEdges(HashSet<Edge<ILinkIdentifier>> edges) {
            if (edges.Count == 0) return;
            var queue = new Queue<ILinkIdentifier>(edges.Select(x => x.Target));
            var source = edges.First().Source;
            var featureFlagService = new FeatureFlagService();
            var essentialQuests = featureFlagService.FeatureFlags.Keys.First().EssentialRecords;
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                if (current != source) {
                    dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(source, current));
                }

                if (!included.Add(current)) continue;
                // logger.Here().Verbose("Retaining {Record} referenced from {Source}", current, source);
                if (!graph.OutgoingEdges.TryGetValue(current, out var currentEdges)) continue;

                foreach (var edge in currentEdges) {
                    if (edge.Target is FormLinkIdentifier formLink2) {
                        if (formLink2.FormLink.Type == typeof(IQuestGetter)
                         && !essentialQuests.Contains(formLink2.FormLink)) {
                            Console.WriteLine($"Retaining non-essential quest {formLink2.FormLink.FormKey} referenced from {current}");
                        }
                    }
                    queue.Enqueue(edge.Target);
                }
            }
        }
    }
}
