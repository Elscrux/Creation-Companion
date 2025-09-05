using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using ModCleaner.Models;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
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

    private readonly IEnumerable<FormLinkInformation> _essentialRecords = essentialRecordProvider.GetEssentialRecords().ToHashSet();

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
                     && !masters.Contains(modKey)
                     && !dependencies.Contains(modKey)) continue;

                    if (currentReference.Type == typeof(ILocationGetter)) continue;
                    if (currentReference.Type == typeof(INavigationMeshInfoMapGetter)) continue;
                    if (currentReference.Type == typeof(IWorldspaceGetter)) continue;

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

        var formLinkInformations = mod.EnumerateMajorRecords()
            .Select(r => r.ToFormLinkInformation())
            .ToHashSet();
        var array = includedLinks.OfType<FormLinkIdentifier>().Select(x => x.FormLink).ToHashSet();
        var x = formLinkInformations.FirstOrDefault(x => x.ToString().Contains("05330B"));
        if (x is not null) {
            Console.WriteLine();
        }
        var y = formLinkInformations.FirstOrDefault(x => x.ToString().Contains("0532FD"));
        if (y is not null) {
            Console.WriteLine();
        }
        var z = formLinkInformations.FirstOrDefault(x => x.ToString().Contains("05DC35"));
        if (z is not null) {
            Console.WriteLine();
        }
        var xx = array.FirstOrDefault(x => x.ToString()!.Contains("05330B"));
        if (xx is not null) {
            Console.WriteLine();
        }
        var yyy = array.Where(x => x.ToString()!.Contains("0532FD")).ToArray();
        var yy = array.FirstOrDefault(x => x.ToString()!.Contains("0532FD"));
        if (yy is not null) {
            Console.WriteLine();
        }
        var zz = array.FirstOrDefault(x => x.ToString()!.Contains("05DC35"));
        if (zz is not null) {
            Console.WriteLine();
        }
        return formLinkInformations
            .Except(array)
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
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        var formLink = formLinkIdentifier.FormLink;
        if (formLink.FormKey.ModKey != mod.ModKey
         || _essentialRecords.Contains(formLink)
         || editorEnvironment.LinkCache.ResolveAllSimpleContexts(formLink).Any(c => dependencies.Contains(c.ModKey))) {
            // Retain overrides of records from other mods
            // Retain records that are essential and all their transitive dependencies
            // Retain placeholder records that are going to be replaced by Creation Club records via patch
            // Retain things that are overridden by dependencies
            included.Add(formLinkIdentifier);
            // var iLinkIdentifier = included.FirstOrDefault(x => x is FormLinkIdentifier a && a.FormLink.FormKey.ToString().Contains("000F0A"))

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
        typeof(IStoryManagerQuestNodeGetter),
        typeof(IStoryManagerBranchNodeGetter),
        typeof(IStoryManagerEventNodeGetter),
        typeof(IDialogBranchGetter),
        typeof(IDialogTopicGetter),
        typeof(IDialogViewGetter),
        typeof(IWorldspaceGetter),
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

                if (formLinkIdentifier.FormLink.FormKey.ToString().Contains("05330B")) {
                    Console.WriteLine("");
                }

                if (formLinkIdentifier.FormLink.FormKey.ToString().Contains("0532FD")) {
                    Console.WriteLine("");
                }
                if (formLinkIdentifier.FormLink.FormKey.ToString().Contains("05DC35")) {
                    Console.WriteLine("");
                }

                if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;
                if (edges.Count == 0) continue;
                if (edges.All(x => !included.Contains(x.Target))) continue;

                included.Add(vertex);

                retainOutgoingEdges(edges);
            }
        }
    }
}

public sealed record FeatureFlag(string Name, ModKey ModKey, List<WorldspacePatch> WorldspacePatches, List<FormLinkInformation> EssentialRecords);
public sealed record WorldspacePatch(FormLinkInformation Worldspace, P2Int BottomLeft, P2Int TopRight);

public interface IFeatureFlagService {
    IReadOnlyDictionary<FeatureFlag, bool> FeatureFlags { get; }
    IEnumerable<FeatureFlag> EnabledFeatureFlags { get; }

    bool IsFeatureEnabled(FeatureFlag featureFlag);
    void SetFeatureEnabled(FeatureFlag featureFlag, bool enabled);
}

public interface IEssentialRecordProvider {
    IEnumerable<FormLinkInformation> GetEssentialRecords();
}

public sealed class EssentialRecordProvider(
    IEditorEnvironment editorEnvironment,
    IFeatureFlagService featureFlagService) : IEssentialRecordProvider {

    public IEnumerable<FormLinkInformation> GetEssentialRecords() {
        foreach (var featureFlag in featureFlagService.EnabledFeatureFlags) {
            foreach (var essentialRecord in featureFlag.EssentialRecords) {
                yield return essentialRecord;
            }

            foreach (var worldspacePatch in featureFlag.WorldspacePatches) {
                if (!worldspacePatch.Worldspace.TryResolve<IWorldspaceGetter>(editorEnvironment.LinkCache, out var worldspace)) continue;

                yield return worldspace.ToFormLinkInformation();

                for (var x = worldspacePatch.BottomLeft.X; x <= worldspacePatch.TopRight.X; x++) {
                    for (var y = worldspacePatch.BottomLeft.Y; y <= worldspacePatch.TopRight.Y; y++) {
                        var cell = worldspace.GetCell(new P2Int(x, y));
                        if (cell is null) continue;

                        yield return cell.ToFormLinkInformation();
                    }
                }
            }
        }
    }
}

public sealed class FeatureFlagService(
    ILogger logger,
    IEditorEnvironment editorEnvironment) : IFeatureFlagService {

    private static readonly Type QuestType = typeof(IQuestGetter);

    private readonly Dictionary<FeatureFlag, bool> _featureFlags = new() {
        {
            new FeatureFlag(
                "Bruma",
                ModKey.FromFileName("BSHeartland.esm"),
                [
                    new WorldspacePatch(
                        new FormLinkInformation(FormKey.Factory("0A764B:BSHeartland.esm"), typeof(IWorldspaceGetter)),
                        new P2Int(5, 40),
                        new P2Int(41, 65)
                    )
                ],
                [
                    new FormLinkInformation(FormKey.Factory("0D2631:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("072648:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E785:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07A9C6:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("079E5F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("067877:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("079E6A:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("079E6E:BSHeartland.esm"), QuestType),
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
                    new FormLinkInformation(FormKey.Factory("06DED8:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DA55:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D781:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D783:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D78E:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0947AC:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DED7:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DED6:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DED5:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DED4:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DED3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D6CC9:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D378D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06FF70:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D2622:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("002089:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CACE:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CAD3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C9B15:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0F0D6A:BSHeartland.esm"), QuestType),
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
                    new FormLinkInformation(FormKey.Factory("0D26A0:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("08ACB3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06D945:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06E77B:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06D946:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DB85:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0781AA:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D7AC:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06C0DE:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0CC0BD:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DECD:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06506C:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07CA96:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D5401:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEC8:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DA56:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEC4:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0FA857:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEC0:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEBF:BSHeartland.esm"), QuestType),
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
                    new FormLinkInformation(FormKey.Factory("06E775:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D263D:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0E591F:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0E5911:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0E5912:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0C8AF9:BSHeartland.esm"), QuestType),
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
                    new FormLinkInformation(FormKey.Factory("06DEAC:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEAB:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DA53:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D790:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D797:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("07D799:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEA4:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DA54:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEA3:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("06DEA2:BSHeartland.esm"), QuestType),
                    new FormLinkInformation(FormKey.Factory("0D5404:BSHeartland.esm"), QuestType),
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
