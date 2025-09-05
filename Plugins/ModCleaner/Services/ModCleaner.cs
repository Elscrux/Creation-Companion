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

    public void Start(ISkyrimModGetter mod, IReadOnlyList<ModKey> dependencies, IDataSource dataSource) {
        // Build graph of records that are connected via references
        var graph = BuildGraph(mod, dependencies);

        var included = FindRetainedRecords(graph, mod, dependencies);
        
        var edges = new StreamWriter(@"C:\Users\nickp\Downloads\edges-test2.csv");
        foreach (var link in included) {
            if (link is not FormLinkIdentifier l) continue;

            if (graph.OutgoingEdges.TryGetValue(link, out var outgoing)) {
                foreach (var edge in outgoing) {
                    if (included.Contains(edge.Target) && edge.Target is FormLinkIdentifier t) {
                        edges.WriteLine($"'{l.FormLink.FormKey} - {l.FormLink.Type}','{t.FormLink.FormKey} - {t.FormLink.Type}'");
                    }
                }
            }
        }
        
        var x = included.OfType<FormLinkIdentifier>().FirstOrDefault(x => x.ToString().Contains("05330B"));
        if (x is not null) {
            Console.WriteLine();
        }
        var y = included.OfType<FormLinkIdentifier>().FirstOrDefault(x => x.ToString().Contains("0532FD")); 
        if (y is not null) {
            Console.WriteLine();
        }
        var z = included.OfType<FormLinkIdentifier>().FirstOrDefault(x => x.ToString().Contains("05DC35")); 
        if (z is not null) {
            Console.WriteLine();
        }

        var recordsToClean = recordCleaner.GetRecordsToClean(included, mod);
        var assetsToClean = assetCleaner.GetAssetsToClean(included, dataSource);
        if (recordsToClean.Any(x => x.FormKey.ToString().Contains("05330B"))) {
            Console.WriteLine();
        }
        if (recordsToClean.Any(x => x.FormKey.ToString().Contains("0532FD"))) {
            Console.WriteLine();
        }
        if (recordsToClean.Any(x => x.FormKey.ToString().Contains("05DC35"))) {
            Console.WriteLine();
        }


        var m = string.Join("\n", assetsToClean.Select(x => x.DataRelativePath.Path));
        logger.Here().Information("Cleaning assets: {Assets}", m);

        // assetCleaner.CleanDataSource(dataSource, assetsToClean);
        recordCleaner.CreatedCleanerOverrideMod(mod, recordsToClean);
    }

    private Graph<ILinkIdentifier, Edge<ILinkIdentifier>> BuildGraph(IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var graph = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();

        recordCleaner.BuildGraph(graph, mod, dependencies);
        assetCleaner.BuildGraph(graph, mod, dependencies);

        return graph;
    }

    private HashSet<ILinkIdentifier> FindRetainedRecords(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies) {
        var included = new HashSet<ILinkIdentifier>();

        foreach (var vertex in graph.Vertices) {
            if (included.Contains(vertex)) continue;

            switch (vertex) {
                case FormLinkIdentifier formLinkIdentifier: {
                    recordCleaner.IncludeLinks(graph, mod, dependencies, formLinkIdentifier, included, RetainOutgoingEdges);
                    break;
                }
                case AssetLinkIdentifier assetLinkIdentifier: {
                    assetCleaner.IncludeLinks(graph, mod, dependencies, assetLinkIdentifier, included, RetainOutgoingEdges);
                    break;
                }
            }
        }

        RecordCleaner.FinalIncludeLinks(graph, included, RetainOutgoingEdges);

        return included;

        void RetainOutgoingEdges(HashSet<Edge<ILinkIdentifier>> edges) {
            var queue = new Queue<ILinkIdentifier>(edges.Select(x => x.Target));
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                if (!included.Add(current)) continue;
                if (!graph.OutgoingEdges.TryGetValue(current, out var currentEdges)) continue;

                foreach (var edge in currentEdges) {
                    queue.Enqueue(edge.Target);
                }
            }
        }
    }
}
