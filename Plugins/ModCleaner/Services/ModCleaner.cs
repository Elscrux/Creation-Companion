using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Serilog;
namespace ModCleaner.Services;

public interface ILinkIdentifier;
public record FormLinkIdentifier(IFormLinkIdentifier FormLink) : ILinkIdentifier;
public record AssetLinkIdentifier(IAssetLinkGetter AssetLink) : ILinkIdentifier {
    public virtual bool Equals(AssetLinkIdentifier? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return AssetLink.Equals(other.AssetLink);
    }
    public override int GetHashCode() {
        // TODO add back once AssetLinkGetter hash code implementation was updated
        return AssetLink.DataRelativePath.GetHashCode();
    }
}

public sealed class ModCleaner(
    ILogger logger,
    IFileSystem fileSystem,
    // IAssetTypeService assetTypeService,
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    IRecordController recordController,
    // AssetCleaner assetCleaner,
    RecordCleaner recordCleaner) {

    public void Start(ISkyrimModGetter mod, IReadOnlyList<ModKey> dependencies) {
        // Build graph of records that are connected via references
        var graph = BuildGraph(mod, dependencies);

        var included = FindRetainedRecords(graph, mod, dependencies);
        var x = included.OfType<FormLinkIdentifier>().Select(x => x.FormLink.FormKey).ToArray();
        if (x.All(x => !x.ToString().Contains("05861C:BSAssets.esm"))) {
            Console.WriteLine();
        }
        if (x.All(x => !x.ToString().Contains("053400"))) {
            Console.WriteLine();
        }

        var recordsToClean = mod.EnumerateMajorRecords()
            .Select(x => x.ToFormLinkInformation())
            .Except(included.OfType<FormLinkIdentifier>().Select(x => x.FormLink))
            .ToArray();

        // const string dir = @"E:\TES\Skyrim\modlists\beyond-skyrim\mods\se-assets";
        // var assetsToClean = fileSystem.Directory
        //     .EnumerateFiles(dir, "*", SearchOption.AllDirectories)
        //     .Select(x => new DataRelativePath(x[(dir.Length + 1)..]))
        //     .Select(dataRelativePath => {
        //         try {
        //             return assetTypeService.GetAssetLink(dataRelativePath);
        //         } catch (Exception) {
        //             return null;
        //         }
        //     })
        //     .WhereNotNull()
        //     .Except(included.OfType<AssetLinkIdentifier>().Select(x => x.AssetLink))
        //     .ToArray();
        //
        // logger.Here().Information("Records to clean:");
        // var m = string.Join("\n", assetsToClean.Select(x => x.DataRelativePath.Path));
        // logger.Here().Information(m);

        // foreach (var assetLinkGetter in assetsToClean) { 
        //     assetController.Delete(assetLinkGetter.DataRelativePath.Path);
        // }

        // CreatedCleanedMod(mod, recordsToClean);
        CreatedCleanerMod(mod, recordsToClean);
    }

    private void CreatedCleanedMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanMod = editorEnvironment.AddNewMutableMod(ModKey.FromFileName(mod.ModKey.Name + "Cleaned.esm"));

        foreach (var record in recordsToClean) {
            cleanMod.Remove(new FormKey(cleanMod.ModKey, record.FormKey.ID));
        }
    }

    private void CreatedCleanerMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanMod = editorEnvironment.AddNewMutableMod(ModKey.FromFileName($"Clean{mod.ModKey.Name}.esp"));

        foreach (var record in recordsToClean) {
            var recordOverride = recordController.GetOrAddOverride(record, cleanMod);
            recordOverride.IsDeleted = true;
        }
    }

    private Graph<ILinkIdentifier, Edge<ILinkIdentifier>> BuildGraph(IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var graph = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();

        recordCleaner.BuildGraph(graph, mod, dependencies);
        // assetCleaner.BuildGraph(graph);

        return graph;
    }

    private static readonly Type[] EssentialRecordTypes = [
        typeof(IWorldspaceGetter),
        typeof(ICellGetter),
        typeof(IPlacedGetter),
        typeof(ILandscapeGetter),
        typeof(INavigationMeshGetter),
        typeof(INavigationMeshInfoMapGetter),
        typeof(IQuestGetter),
        typeof(IDialogBranchGetter),
        typeof(IDialogResponsesGetter),
        typeof(IStoryManagerEventNodeGetter),
        typeof(IStoryManagerBranchNodeGetter),
        typeof(IStoryManagerQuestNodeGetter),
        typeof(ILoadScreenGetter),
    ];

    private static readonly Type[] ImplicitRetainedRecordTypes = [
        typeof(IConstructibleObjectGetter),
        typeof(IRelationshipGetter),
        typeof(IDialogViewGetter),
    ];

    private static readonly Type[] SelfRetainedRecordTypes = [
        typeof(IIdleAnimationGetter),
        typeof(IAddonNodeGetter),
        typeof(IAnimatedObjectGetter),
    ];

    private HashSet<ILinkIdentifier> FindRetainedRecords(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        ISkyrimModGetter mod,
        IReadOnlyList<ModKey> dependencies) {
        var included = new HashSet<ILinkIdentifier>();

        foreach (var vertex in graph.Vertices) {
            if (included.Contains(vertex)) continue;

            if (vertex is FormLinkIdentifier formLinkIdentifier) {
                var formLink = formLinkIdentifier.FormLink;
                if (formLink.FormKey.ModKey != mod.ModKey
                 || formLink.Type.InheritsFromAny(EssentialRecordTypes)
                 || editorEnvironment.LinkCache.ResolveAllSimpleContexts(formLink).Any(c => dependencies.Contains(c.ModKey))) {
                    // Retain overrides of records from other mods
                    // Retain records that are essential and all their transitive dependencies
                    // Retain placeholder records that are going to be replaced by Creation Club records via patch
                    // Retain things that are overridden by dependencies
                    included.Add(vertex);

                    if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;

                    RetainOutgoingEdges(edges);
                } else if (formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)) {
                    // Retain records that are self-retained, and keep adding all other records that are linked to them
                    var queue = new Queue<ILinkIdentifier>([vertex]);
                    while (queue.Count > 0) {
                        var current = queue.Dequeue();
                        if (!included.Add(current)) continue;
                        if (!graph.IncomingEdges.TryGetValue(current, out var currentEdges)) continue;

                        queue.Enqueue(currentEdges.Select(x => x.Target)
                            .Where(t => formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)));
                    }
                } else if (formLink.Type == typeof(IDialogTopicGetter)) {
                    // Only custom and scene dialog topics can be unused, everything else is implicitly retained
                    var record = editorEnvironment.LinkCache.Resolve<IDialogTopicGetter>(formLink.FormKey);
                    if (record.SubtypeName.Type is not "CUST" and not "SCEN") {
                        included.Add(vertex);

                        if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;

                        RetainOutgoingEdges(edges);
                    }
                } else if (formLink.Type == typeof(ISceneGetter)) {
                    // Retain scenes that begin on quest start
                    var record = editorEnvironment.LinkCache.Resolve<ISceneGetter>(formLink.FormKey);
                    if (record.Flags is not null && record.Flags.Value.HasFlag(Scene.Flag.BeginOnQuestStart)) {
                        included.Add(vertex);

                        if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;

                        RetainOutgoingEdges(edges);
                    }
                }
            }
        }

        // Retain records that link to any records that are retained
        // These records don't retain any other records implicitly in the current selection
        foreach (var implicitType in ImplicitRetainedRecordTypes) {
            foreach (var vertex in graph.Vertices) {
                if (vertex is not FormLinkIdentifier formLinkIdentifier) continue;
                if (formLinkIdentifier.FormLink.Type != implicitType) continue;

                if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;
                if (edges.Count == 0) continue;
                if (edges.All(x => !included.Contains(x.Target))) continue;

                included.Add(vertex);

                RetainOutgoingEdges(edges);
            }
        }

        return included;

        void RetainOutgoingEdges(HashSet<Edge<ILinkIdentifier>> edges) {
            var queue = new Queue<ILinkIdentifier>(edges.Select(x => x.Target));
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                switch (current) {
                    case AssetLinkIdentifier assetLinkIdentifier when assetLinkIdentifier.AssetLink.DataRelativePath.Path.Contains("AmuletOfMara_d"): Console.WriteLine();
                        break;
                    case AssetLinkIdentifier assetLinkIdentifier: {
                        if (assetLinkIdentifier.AssetLink.DataRelativePath.Path.Contains("AmuletOfMara_GO")) {
                            Console.WriteLine();
                            var pair = graph.OutgoingEdges.FirstOrDefault(x => x.Key is AssetLinkIdentifier a && a.AssetLink.DataRelativePath.Path.Contains("AmuletOfMara_GO"));
                            Console.WriteLine(pair.Key);
                            Console.WriteLine(pair.Value);
                            Console.WriteLine(pair.Key.GetHashCode());
                            Console.WriteLine(assetLinkIdentifier.GetHashCode());
                        }
                        break;
                    }
                    case FormLinkIdentifier formLinkIdentifier:
                        // Guar package
                        if (formLinkIdentifier.FormLink.FormKey.ToString().Contains("05861C")) {
                            Console.WriteLine();
                        }
                        // Knots
                        if (formLinkIdentifier.FormLink.FormKey.ToString().Contains("053400")) {
                            Console.WriteLine();
                        }
                        break;
                }
                if (!included.Add(current)) continue;
                if (!graph.OutgoingEdges.TryGetValue(current, out var currentEdges)) continue;

                foreach (var edge in currentEdges) {
                    queue.Enqueue(edge.Target);
                }
            }
        }
    }
}
