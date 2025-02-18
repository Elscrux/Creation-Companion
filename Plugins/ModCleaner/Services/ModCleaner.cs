using CreationEditor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Definitions;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace ModCleaner.Services;

public sealed class ModCleaner(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    IRecordController recordController,
    IRecordReferenceController recordReferenceController) {

    public void Start(ISkyrimModGetter mod, IReadOnlyList<ModKey> dependencies) {
        // Build graph of records that are connected via references
        var graph = BuildGraph(mod, dependencies);

        var included = FindRetainedRecords(graph, dependencies);

        var recordsToClean = mod.EnumerateMajorRecords()
            .Select(x => x.ToFormLinkInformation())
            .Except(included)
            .ToArray();

        // CreatedCleanedMod(mod, recordsToClean);
        CreatedCleanerMod(mod, recordsToClean);
    }

    private void CreatedCleanedMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanModKey = ModKey.FromFileName(mod.ModKey.Name + "Cleaned.esm");
        var cleanMod = new SkyrimMod(
            cleanModKey,
            editorEnvironment.GameEnvironment.GameRelease.ToSkyrimRelease(),
            IEditorEnvironment.DefaultModVersion);

        foreach (var record in recordsToClean) {
            cleanMod.Remove(new FormKey(cleanMod.ModKey, record.FormKey.ID));
        }

        editorEnvironment.AddMutableMod(cleanMod);
    }

    private void CreatedCleanerMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanMod = new SkyrimMod(
            ModKey.FromFileName($"Clean{mod.ModKey.Name}.esp"),
            editorEnvironment.GameEnvironment.GameRelease.ToSkyrimRelease(),
            IEditorEnvironment.DefaultModVersion);

        foreach (var record in recordsToClean) {
            var recordOverride = recordController.GetOrAddOverride(record, cleanMod);
            recordOverride.IsDeleted = true;
        }

        editorEnvironment.AddMutableMod(cleanMod);
    }

    private Graph<IFormLinkIdentifier, Edge<IFormLinkIdentifier>> BuildGraph(IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var graph = new Graph<IFormLinkIdentifier, Edge<IFormLinkIdentifier>>();

        foreach (var record in mod.EnumerateMajorRecords()) {
            // Add all transitive dependencies of the record
            var queue = new Queue<IFormLinkIdentifier>([record.ToFormLinkInformation()]);
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                graph.AddVertex(current);

                foreach (var currentReference in recordReferenceController.GetReferences(current.FormKey)) {
                    // This just checks if the reference was defined in the mod or one of its dependencies. Update if needed.
                    var modKey = currentReference.FormKey.ModKey;
                    if (modKey != mod.ModKey
                     && !dependencies.Contains(modKey)) continue;

                    if (!graph.Vertices.Contains(currentReference)) {
                        queue.Enqueue(currentReference);
                    }

                    graph.AddEdge(new Edge<IFormLinkIdentifier>(currentReference, current));
                }
            }
        }

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
    ];

    private static readonly Type[] ImplicitRetainedRecordTypes = [
        typeof(IConstructibleObjectGetter),
        typeof(IRelationshipGetter),
        typeof(IDialogViewGetter),
        typeof(IStoryManagerQuestNodeGetter),
    ];

    private static readonly Type[] SelfRetainedRecordTypes = [
        typeof(IIdleAnimationGetter),
        typeof(ILoadScreenGetter),
        typeof(IAddonNodeGetter),
        typeof(IAnimatedObjectGetter),
    ];

    private HashSet<IFormLinkIdentifier> FindRetainedRecords(
        Graph<IFormLinkIdentifier, Edge<IFormLinkIdentifier>> graph,
        IReadOnlyList<ModKey> dependencies) {
        var included = new HashSet<IFormLinkIdentifier>();

        foreach (var vertex in graph.Vertices) {
            if (included.Contains(vertex)) continue;

            if (SkyrimDefinitions.SkyrimModKeys.Contains(vertex.FormKey.ModKey)
             || vertex.Type.InheritsFromAny(EssentialRecordTypes) 
             || editorEnvironment.LinkCache.ResolveAllSimpleContexts(vertex).Any(c => dependencies.Contains(c.ModKey))) {
                // Retain vanilla overrides
                // Retain records that are essential and all their transitive dependencies
                // Retain placeholder records that are going to be replaced by Creation Club records via patch
                // Retain things that are overridden by dependencies
                included.Add(vertex);

                if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;

                var queue = new Queue<IFormLinkIdentifier>(edges.Select(x => x.Target));
                while (queue.Count > 0) {
                    var current = queue.Dequeue();
                    if (!included.Add(current)) continue;
                    if (!graph.OutgoingEdges.TryGetValue(current, out var currentEdges)) continue;

                    foreach (var edge in currentEdges) {
                        queue.Enqueue(edge.Target);
                    }
                }
            } else if (vertex.Type.InheritsFromAny(SelfRetainedRecordTypes)) {
                // Retain records that are self-retained, and keep adding all other records that are linked to them
                var queue = new Queue<IFormLinkIdentifier>([vertex]);
                while (queue.Count > 0) {
                    var current = queue.Dequeue();
                    if (!included.Add(current)) continue;
                    if (!graph.IncomingEdges.TryGetValue(current, out var currentEdges)) continue;

                    queue.Enqueue(currentEdges.Select(x => x.Target)
                        .Where(t => t.Type.InheritsFromAny(SelfRetainedRecordTypes)));
                }
            } else if (vertex.Type == typeof(IDialogTopicGetter)) {
                // Only custom and scene dialog topics can be unused, everything else is implicitly retained
                var record = editorEnvironment.LinkCache.Resolve<IDialogTopicGetter>(vertex.FormKey);
                if (record.SubtypeName.Type is not "CUST" and not "SCEN") {
                    included.Add(vertex);
                }
            }
        }

        // Retain records that link to any records that are retained
        // These records don't retain any other records implicitly in the current selection
        foreach (var implicitType in ImplicitRetainedRecordTypes) {
            foreach (var vertex in graph.Vertices.Where(x => x.Type == implicitType)) {
                if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;
                if (edges.Count == 0) continue;
                if (edges.All(x => !included.Contains(x.Target))) continue;

                included.Add(vertex);
            }
        }

        return included;
    }
}
