using CreationEditor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using ModCleaner.Models;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.Services;

public sealed class RecordCleaner(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    IRecordController recordController,
    IReferenceService referenceService) {

    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var masters = mod.GetTransitiveMasters(editorEnvironment.GameEnvironment).ToArray();
        foreach (var record in mod.EnumerateMajorRecords()) {
            // Add all transitive dependencies of the record
            var queue = new Queue<IFormLinkIdentifier>([record.ToFormLinkInformation()]);
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                graph.AddVertex(new FormLinkIdentifier(current));

                foreach (var currentReference in referenceService.GetRecordReferences(current)) {
                    // This just checks if the reference was defined in the mod or one of its dependencies. Update if needed.
                    var modKey = currentReference.FormKey.ModKey;
                    if (modKey != mod.ModKey
                     && !masters.Contains(modKey)
                     && !dependencies.Contains(modKey)) continue;

                    var currentReferenceLink = new FormLinkIdentifier(currentReference);
                    if (!graph.Vertices.Contains(currentReferenceLink)) {
                        queue.Enqueue(currentReference);
                    }

                    graph.AddEdge(new Edge<ILinkIdentifier>(currentReferenceLink, new FormLinkIdentifier(current)));
                }
            }
        }
    }

    public static IReadOnlyList<IFormLinkIdentifier> GetRecordsToClean(
        HashSet<ILinkIdentifier> includedLinks,
        IModGetter mod) {

        return mod.EnumerateMajorRecords()
            .Select(r => r.ToFormLinkInformation())
            .Except(includedLinks.OfType<FormLinkIdentifier>().Select(x => x.FormLink))
            .ToArray();
    }

    public void CreatedCleanedMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanMod = editorEnvironment.AddNewMutableMod(ModKey.FromFileName(mod.ModKey.Name + "Cleaned.esm"));

        foreach (var record in recordsToClean) {
            cleanMod.Remove(new FormKey(cleanMod.ModKey, record.FormKey.ID));
        }
    }

    public void CreatedCleanerOverrideMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanMod = editorEnvironment.AddNewMutableMod(ModKey.FromFileName($"Clean{mod.ModKey.Name}.esp"));

        foreach (var record in recordsToClean) {
            var recordOverride = recordController.GetOrAddOverride(record, cleanMod);
            recordOverride.IsDeleted = true;
        }
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

    private static readonly Type[] SelfRetainedRecordTypes = [
        typeof(IIdleAnimationGetter),
        typeof(IAddonNodeGetter),
        typeof(IAnimatedObjectGetter),
    ];

    public void IncludeLinks(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies,
        FormLinkIdentifier formLinkIdentifier,
        HashSet<ILinkIdentifier> included,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        var formLink = formLinkIdentifier.FormLink;
        if (formLink.FormKey.ModKey != mod.ModKey
         || formLink.Type.InheritsFromAny(EssentialRecordTypes)
         || editorEnvironment.LinkCache.ResolveAllSimpleContexts(formLink).Any(c => dependencies.Contains(c.ModKey))) {
            // Retain overrides of records from other mods
            // Retain records that are essential and all their transitive dependencies
            // Retain placeholder records that are going to be replaced by Creation Club records via patch
            // Retain things that are overridden by dependencies
            included.Add(formLinkIdentifier);

            if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) return;

            retainOutgoingEdges(edges);
        } else if (formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)) {
            // Retain records that are self-retained, and keep adding all other records that are linked to them
            var queue = new Queue<ILinkIdentifier>([formLinkIdentifier]);
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
                included.Add(formLinkIdentifier);

                if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) return;

                retainOutgoingEdges(edges);
            }
        } else if (formLink.Type == typeof(ISceneGetter)) {
            // Retain scenes that begin on quest start
            var record = editorEnvironment.LinkCache.Resolve<ISceneGetter>(formLink.FormKey);
            if (record.Flags is not null && record.Flags.Value.HasFlag(Scene.Flag.BeginOnQuestStart)) {
                included.Add(formLinkIdentifier);

                if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) return;

                retainOutgoingEdges(edges);
            }
        }
    }

    private static readonly Type[] ImplicitRetainedRecordTypes = [
        typeof(IConstructibleObjectGetter),
        typeof(IRelationshipGetter),
        typeof(IDialogViewGetter),
    ];

    public static void FinalIncludeLinks(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        HashSet<ILinkIdentifier> included,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
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

                retainOutgoingEdges(edges);
            }
        }
    }
}
