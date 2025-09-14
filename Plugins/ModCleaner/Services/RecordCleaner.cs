using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using DynamicData;
using ModCleaner.Models;
using ModCleaner.Services;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.IO;
using Serilog;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.Services;

public sealed class RecordCleaner(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    ILogger logger,
    IEssentialRecordProvider essentialRecordProvider,
    IRecordController recordController,
    IAssetTypeService assetTypeService,
    IReferenceService referenceService) {

    private readonly Lazy<IReadOnlySet<FormLinkInformation>> _essentialRecords = new(() => essentialRecordProvider.GetEssentialRecords().ToHashSet());

    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var masters = mod.GetTransitiveMasters(editorEnvironment.GameEnvironment).ToArray();
        var processedRecords = new HashSet<FormKey>();
        foreach (var record in mod.EnumerateMajorRecords()) {
            if (record is ICellGetter cell) {
                var cellIdentifier = new FormLinkIdentifier(cell.ToFormLinkInformation());
                graph.AddVertex(cellIdentifier);

                foreach (var placed in cell.Temporary.Concat(cell.Persistent)) {
                    var placedIdentifier = new FormLinkIdentifier(new FormLinkInformation(placed.FormKey, typeof(IPlacedGetter)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(cellIdentifier, placedIdentifier));
                }

                if (cell.Landscape is not null) {
                    var landscapeIdentifier = new FormLinkIdentifier(new FormLinkInformation(cell.Landscape.FormKey, typeof(ILandscapeGetter)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(cellIdentifier, landscapeIdentifier));
                }

                foreach (var navmesh in cell.NavigationMeshes) {
                    var navmeshIdentifier = new FormLinkIdentifier(new FormLinkInformation(navmesh.FormKey, typeof(INavigationMeshGetter)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(cellIdentifier, navmeshIdentifier));
                }
            }

            if (record is IDialogTopicGetter topic) {
                var topicIdentifier = new FormLinkIdentifier(topic.ToFormLinkInformation());
                graph.AddVertex(topicIdentifier);

                foreach (var responses in topic.Responses) {
                    var responsesIdentifier = new FormLinkIdentifier(new FormLinkInformation(responses.FormKey, typeof(IDialogResponsesGetter)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(topicIdentifier, responsesIdentifier));
                }
            }

            // Add all transitive dependencies of the record
            var queue = new Queue<IFormLinkIdentifier>([record.ToFormLinkInformation()]);
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                if (!processedRecords.Add(current.FormKey)) {
                    continue;
                }

                graph.AddVertex(new FormLinkIdentifier(current));

                foreach (var currentReference in referenceService.GetRecordReferences(current)) {
                    // This just checks if the reference was defined in the mod or one of its dependencies. Update if needed.
                    var modKey = currentReference.FormKey.ModKey;
                    if (modKey != mod.ModKey
                        // && !masters.Contains(modKey) // If it is part of a master we're not interested in it further, it will never be cleaned and it can't reference something from our mod that might need to be cleaned
                     && !dependencies.Contains(modKey)) continue;

                    // Remove these connections and fix them manually
                    // Removing references from location to something - potentially not needed (clearing crime factions list might have fixed it) / too broad
                    if (currentReference.Type == typeof(ILocationGetter)) continue;

                    // Is regenerated
                    if (currentReference.Type == typeof(INavigationMeshInfoMapGetter)) continue;

                    // Removing references from worldspaces to something like large refs or all recursive nodes from cells etc
                    if (currentReference.Type == typeof(IWorldspaceGetter)) continue;

                    // Navmesh to navmesh links will connect all cells in the worldspace which we don't want - re-finalize navmesh after cleaning!
                    if (current.Type == typeof(INavigationMeshGetter) && currentReference.Type == typeof(INavigationMeshGetter)) continue;

                    var currentReferenceLink = new FormLinkIdentifier(currentReference);
                    if (!graph.Vertices.Contains(currentReferenceLink)) {
                        queue.Enqueue(currentReference);
                    }

                    graph.AddEdge(new Edge<ILinkIdentifier>(currentReferenceLink, new FormLinkIdentifier(current)));
                }

                foreach (var assetReference in referenceService.GetAssetReferences(current)) {
                    try {
                        var assetLink = assetTypeService.GetAssetLink(assetReference);
                        if (assetLink is null) continue;

                        var assetLinkIdentifier = new AssetLinkIdentifier(assetLink);
                        if (!graph.Vertices.Contains(assetLinkIdentifier)) {
                            graph.AddVertex(assetLinkIdentifier);
                        }

                        graph.AddEdge(new Edge<ILinkIdentifier>(assetLinkIdentifier, new FormLinkIdentifier(current)));
                    } catch (Exception e) {
                        // Log the error but continue processing other records
                        logger.Here().Error(e, "Error creating asset link for {Asset}", assetReference);
                    }
                }
            }
        }
    }

    public IReadOnlyList<IFormLinkIdentifier> GetRecordsToClean(
        HashSet<ILinkIdentifier> includedLinks,
        IModGetter mod) {
        return mod.EnumerateMajorRecords()
            .Select(r => r.ToFormLinkInformation())
            .Except(includedLinks
                .OfType<FormLinkIdentifier>()
                .Select(x => x.FormLink))
            .ToArray();
    }

    public void CreatedCleanedMod(ISkyrimModGetter mod, IReadOnlyList<IFormLinkIdentifier> recordsToClean) {
        var oldModKey = mod.ModKey;
        var fileSystem = new FileSystem();
        using var tmp = TempFolder.Factory();
        var fileSystemRoot = tmp.Dir;
        var oldModPath = new ModPath(oldModKey, fileSystem.Path.Combine(fileSystemRoot, oldModKey.FileName.String));

        // Write mod to file system
        mod.WriteToBinary(oldModPath,
            BinaryWriteParameters.Default with {
                FileSystem = fileSystem
            });

        // Rename mod file
        var newModKey = ModKey.FromFileName("Clean" + mod.ModKey.FileName);
        var newModPath = new ModPath(newModKey, fileSystem.Path.Combine(fileSystemRoot, newModKey.FileName.String));
        fileSystem.File.Move(oldModPath, newModPath);

        // Read renamed mod as new mod
        var duplicate = ModInstantiator.ImportSetter(newModPath,
            mod.GameRelease,
            BinaryReadParameters.Default with {
                FileSystem = fileSystem
            });
        // return duplicateInto;
        // var duplicate = mod.Duplicate(ModKey.FromFileName("Clean" + mod.ModKey.FileName));

        for (var index = 0; index < recordsToClean.Count; index++) {
            var record = recordsToClean[index];
            Console.WriteLine(index + " " + record.Type.Name);
            duplicate.Remove(new FormKey(duplicate.ModKey, record.FormKey.ID), record.Type);
        }

        editorEnvironment.Update(updater => updater
            .LoadOrder.AddMutableMods(duplicate)
            .Build());
    }

    public void CreatedCleanerOverrideMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanMod = editorEnvironment.AddNewMutableMod(ModKey.FromFileName($"Clean{mod.ModKey.Name}.esp"));

        var placedObjectType = typeof(IPlacedObjectGetter);
        var placedType = typeof(IPlacedGetter);
        foreach (var record in recordsToClean) {
            if (record.Type == placedObjectType) continue; // TODO: temporary to make this faster
            if (record.Type == placedType) continue; // TODO: temporary to make this faster

            // if (record.Type == typeof(ILandscapeGetter)) continue; // TODO: temporary to make this faster
            // if (record.Type == typeof(INavigationMeshGetter)) continue; // TODO: temporary to make this faster
            var recordOverride = recordController.GetOrAddOverride(record, cleanMod);
            recordOverride.IsDeleted = true;
        }
    }

    private static readonly Type[] EssentialRecordTypes = [
        typeof(ICellGetter),
        typeof(IQuestGetter),
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
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> dependencyGraph,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        var formLink = formLinkIdentifier.FormLink;
        if (formLink.FormKey.ModKey != mod.ModKey
         || _essentialRecords.Value.Contains(formLink)
         || (dependencies.Count > 0 && editorEnvironment.LinkCache.ResolveAllSimpleContexts(formLink).Any(c => dependencies.Contains(c.ModKey)))) {
            // Retain overrides of records from other mods
            // Retain records that are essential and all their transitive dependencies
            // Retain placeholder records that are going to be replaced by Creation Club records via patch
            // Retain things that are overridden by dependencies
            included.Add(formLinkIdentifier);
            dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(formLinkIdentifier, formLinkIdentifier));

            if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) return;

            retainOutgoingEdges(edges);
        } else if (formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)) {
            // Retain records that are self-retained, and keep adding all other records that are linked to them
            var queue = new Queue<ILinkIdentifier>([formLinkIdentifier]);
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(formLinkIdentifier, formLinkIdentifier));
                if (!included.Add(current)) continue;
                if (!graph.IncomingEdges.TryGetValue(current, out var currentEdges)) continue;

                queue.Enqueue(currentEdges.Select(x => x.Target)
                    .Where(t => formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)));
            }
        }
        // else if (formLink.Type == typeof(IDialogTopicGetter)) {
        //     // Only custom and scene dialog topics can be unused, everything else is implicitly retained
        //     var record = editorEnvironment.LinkCache.Resolve<IDialogTopicGetter>(formLink.FormKey);
        //     if (record.SubtypeName.Type is not "CUST" and not "SCEN") {
        //         included.Add(formLinkIdentifier);
        // 
        //         if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) return;
        // 
        //         retainOutgoingEdges(edges);
        //     }
        // } 
    }

    /// <summary>
    /// If what I link to is retained, I am also retained.
    /// </summary>
    private static readonly Type[] ImplicitRetainedRecordTypes = [
        typeof(IConstructibleObjectGetter),
        typeof(IRelationshipGetter),
        typeof(IStoryManagerQuestNodeGetter),
        typeof(IDialogViewGetter),
    ];
    // typeof(IStoryManagerQuestNodeGetter),
    // typeof(IStoryManagerBranchNodeGetter),
    // typeof(IStoryManagerEventNodeGetter),
    // typeof(IDialogBranchGetter),
    // typeof(IDialogTopicGetter),
    // typeof(IWorldspaceGetter),

    public void FinalIncludeLinks(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        HashSet<ILinkIdentifier> included,
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> dependencyGraph,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        // Retain records that link to any records that are retained
        // These records don't retain any other records implicitly in the current selection
        foreach (var implicitType in ImplicitRetainedRecordTypes) {
            foreach (var vertex in graph.Vertices) {
                if (vertex is not FormLinkIdentifier formLinkIdentifier) continue;
                if (formLinkIdentifier.FormLink.Type != implicitType) continue;
                if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;

                if (formLinkIdentifier.FormLink.Type == typeof(IStoryManagerQuestNodeGetter)) {
                    edges = edges
                        .Where(x => x.Target is FormLinkIdentifier f
                         && f.FormLink.Type != typeof(IStoryManagerQuestNodeGetter)
                         && f.FormLink.Type != typeof(IStoryManagerBranchNodeGetter))
                        .ToHashSet();
                }

                if (edges.Count == 0) continue;

                // Changed to Any() instead of All() - probably a problem? Explain this choice in a comment
                if (edges.Any(x => !included.Contains(x.Target))) continue;

                // Keep parent nodes of quest nodes
                if (formLinkIdentifier.FormLink.Type == typeof(IStoryManagerQuestNodeGetter)
                 && editorEnvironment.LinkCache.TryResolve<IStoryManagerQuestNodeGetter>(formLinkIdentifier.FormLink.FormKey, out var questNode)) {
                    var parentNode = questNode.Parent.TryResolve(editorEnvironment.LinkCache);
                    while (parentNode is not null) {
                        included.Add(new FormLinkIdentifier(parentNode));
                        dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(vertex, new FormLinkIdentifier(parentNode)));
                        parentNode = parentNode.Parent.TryResolve(editorEnvironment.LinkCache);
                    }
                }

                included.Add(vertex);

                retainOutgoingEdges(edges);
            }
        }

        foreach (var vertex in graph.Vertices) {
            if (vertex is not FormLinkIdentifier { FormLink: var formLink } formLinkIdentifier) continue;

            if (formLink.Type == typeof(ISceneGetter)) {
                // Retain scenes that begin on quest start                                                                       

                var scene = editorEnvironment.LinkCache.Resolve<ISceneGetter>(formLink.FormKey);
                if (scene.Flags is null || !scene.Flags.Value.HasFlag(Scene.Flag.BeginOnQuestStart)) continue;
                if (!included.Contains(new FormLinkIdentifier(scene.Quest))) continue;

                included.Add(formLinkIdentifier);

                if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) continue;

                retainOutgoingEdges(edges);
            } else if (formLink.Type == typeof(IDialogTopicGetter)) {
                // Only custom and scene dialog topics can be unused, everything else is implicitly retained
                var topic = editorEnvironment.LinkCache.Resolve<IDialogTopicGetter>(formLink.FormKey);
                if (topic.SubtypeName.Type is not "CUST" and not "SCEN") {
                    if (!included.Contains(new FormLinkIdentifier(topic.Quest))) continue;

                    included.Add(formLinkIdentifier);

                    if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) continue;

                    retainOutgoingEdges(edges);
                }
            }
        }
    }
}

// TODO: OR USE Regions instead of patches?
public sealed record FeatureFlag(
    string Name,
    ModKey ModKey,
    Dictionary<IFormLinkGetter<IWorldspaceGetter>, List<IFormLinkGetter<IRegionGetter>>> AllowedRegions,
    List<FormLinkInformation> EssentialRecords);

public sealed record WorldspacePatch(FormLinkInformation Worldspace, P2Int BottomLeft, P2Int TopRight);

public interface IFeatureFlagService {
    IReadOnlyDictionary<FeatureFlag, bool> FeatureFlags { get; }
    IEnumerable<FeatureFlag> EnabledFeatureFlags { get; }

    bool IsFeatureEnabled(FeatureFlag featureFlag);
    void SetFeatureEnabled(FeatureFlag featureFlag, bool enabled);
}

public interface IEssentialRecordProvider {
    IEnumerable<FormLinkInformation> GetEssentialRecords();
    bool IsInvalidExteriorCell(IFormLinkGetter<IWorldspaceGetter> worldspace, ICellGetter cell);
}

public sealed class EssentialRecordProvider(
    IEditorEnvironment editorEnvironment,
    IFeatureFlagService featureFlagService) : IEssentialRecordProvider {

    public IEnumerable<FormLinkInformation> GetEssentialRecords() {
        foreach (var featureFlag in featureFlagService.EnabledFeatureFlags) {
            foreach (var essentialRecord in featureFlag.EssentialRecords) {
                yield return essentialRecord;
            }

            foreach (var (worldspaceLink, regions) in featureFlag.AllowedRegions) {
                if (!worldspaceLink.TryResolve(editorEnvironment.LinkCache, out var worldspace)) continue;

                yield return worldspace.ToFormLinkInformation();

                foreach (var cell in worldspace.EnumerateCells()
                    .Where(c => c.Regions is not null && c.Regions.Intersect(regions).Any())) {
                    yield return cell.ToFormLinkInformation();
                }
            }
        }
    }

    public bool IsInvalidExteriorCell(IFormLinkGetter<IWorldspaceGetter> worldspace, ICellGetter cell) {
        var allowedRegions = featureFlagService.EnabledFeatureFlags
            .SelectMany(f => f.AllowedRegions.TryGetValue(worldspace, out var allowedRegions) ? allowedRegions : [])
            .ToArray();

        // When there is no reference of the worldspace in any feature flag, all cells are valid
        if (allowedRegions.Length == 0) return false;

        return cell.Regions is null || !cell.Regions.Intersect(allowedRegions).Any();
    }
}

public sealed class FeatureFlagService : IFeatureFlagService {

    private static readonly Type QuestType = typeof(IQuestGetter);

    private readonly Dictionary<FeatureFlag, bool> _featureFlags = new() {
        {
            new FeatureFlag(
                "Bruma",
                ModKey.FromFileName("BSHeartland.esm"),
                new Dictionary<IFormLinkGetter<IWorldspaceGetter>, List<IFormLinkGetter<IRegionGetter>>> {
                    {
                        new FormLink<IWorldspaceGetter>(FormKey.Factory("0A764B:BSHeartland.esm")),
                        [
                            new FormLink<IRegionGetter>(FormKey.Factory("0CBCDD:BSHeartland.esm")),
                        ]
                    }
                },
                [
                    new FormLinkInformation(FormKey.Factory("072648:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E785:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07A9C6:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("079E5F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("067877:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("079E6A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("079E6E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0E7CC8:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("003A57:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3796:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("003A55:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07F0FB:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D6E80:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D6996:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("02DB52:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0650C3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("065163:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06516C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0650BA:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0723A2:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E76F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072639:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07F00A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07F00B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06F999:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("08652B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D54E6:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("086531:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DB5F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D36E2:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D36E1:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3579:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("003A58:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("003A5A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C7E4C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C7E4A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("047FAA:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("067841:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06783D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("08AE84:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("08BF0C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("086207:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3F45:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("086208:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0CC03E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("08650F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("086509:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D5682:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07F0FA:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9BE1:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3820:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06513C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CAAF:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06F9B8:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DA55:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D781:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D783:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D78E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0947AC:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06FF70:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("002089:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CACE:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CAD3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("003A54:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07434C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D6CCA:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06505D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("065050:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07434E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06507B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E776:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E77F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07434D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07260E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E784:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E7A2:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("078172:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("065100:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06504E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("003A59:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D6CC4:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("02DB20:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E7A8:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E7A9:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07434F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072609:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C70:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07260C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C66:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C6A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C6C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C59:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C60:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C62:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C64:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07260A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C48:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C57:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072612:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072615:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07260B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07260F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072614:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072610:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07260D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C4A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C4C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C4E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C50:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C72:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C6E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C53:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C55:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072608:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072613:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072611:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0650B7:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C46:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C35:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07434B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06D961:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("00196C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("08BEF5:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("078168:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("08ACB3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06D945:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E77B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DB85:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0781AA:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D7AC:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06C0DE:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0CC0BD:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DECD:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06506C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CA96:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D5401:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DA56:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07456C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("063C6F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07456B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C88E3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0CE0F1:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0CE135:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("083953:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("083A17:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0CC08B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("05BBD1:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0700DF:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("063C6C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0EAF69:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0700E0:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0AF307:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("077E84:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("063C6D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("05BC0C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06F9A1:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0882F6:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3F33:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A3C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06F938:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0723B3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("073C23:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("001961:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEB5:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEB4:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEB1:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3740:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0650CD:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0B057E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9B41:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9B1A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3F2C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E78C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06F9A9:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0650D4:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CADF:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DA53:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D790:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D797:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D799:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0B2257:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("094805:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C7E6D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C7E65:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C88F1:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C890B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A10:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A14:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A2C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A24:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A6B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A31:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9CA9:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A50:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0E5BDB:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A51:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A4F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A6C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9CC3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9CC6:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C8902:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A46:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A54:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A56:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9CAF:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3788:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9CB8:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D370E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9CBD:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C8908:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C8909:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C890A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("063C6E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06EDF8:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06EDF6:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9A88:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06507E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D3740:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("20ED0C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("20ED0B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("20ED0A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("16EB40:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("1C622C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("1C622D:BSHeartland.esm"), QuestType),
                ]),
            true
        }
    };

    public IReadOnlyDictionary<FeatureFlag, bool> FeatureFlags => _featureFlags;

    public IEnumerable<FeatureFlag> EnabledFeatureFlags => FeatureFlags.Where(kv => kv.Value).Select(kv => kv.Key);

    public bool IsFeatureEnabled(FeatureFlag featureFlag) {
        return FeatureFlags.GetValueOrDefault(featureFlag, false);
    }
    public void SetFeatureEnabled(FeatureFlag featureFlag, bool enabled) {
        _featureFlags[featureFlag] = enabled;
    }
};
