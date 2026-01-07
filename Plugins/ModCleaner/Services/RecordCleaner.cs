using System.IO.Abstractions;
using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Notification;
using CreationEditor.Skyrim;
using ModCleaner.Models;
using ModCleaner.Services.FeatureFlag;
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
    IFeatureFlagService featureFlagService,
    IEssentialRecordProvider essentialRecordProvider,
    IRecordController recordController,
    IAssetTypeService assetTypeService,
    IReferenceService referenceService,
    INotificationService notificationService) {

    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies) {
        var masters = mod.GetTransitiveMasters(editorEnvironment.GameEnvironment).ToArray();
        var processedRecords = new HashSet<FormKey>();
        foreach (var record in mod.EnumerateMajorRecords()) {
            // Record specific pre-processing
            switch (record) {
                // Add links directly coming from a worldspace 
                case IWorldspaceGetter worldspace:
                    var link = new FormLinkIdentifier(worldspace.ToFormLinkInformation());
                    graph.AddEdge(new Edge<ILinkIdentifier>(link, new FormLinkIdentifier(worldspace.Climate)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(link, new FormLinkIdentifier(worldspace.EncounterZone)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(link, new FormLinkIdentifier(worldspace.InteriorLighting)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(link, new FormLinkIdentifier(worldspace.Location)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(link, new FormLinkIdentifier(worldspace.LodWater)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(link, new FormLinkIdentifier(worldspace.Music)));
                    graph.AddEdge(new Edge<ILinkIdentifier>(link, new FormLinkIdentifier(worldspace.Water)));
                    break;
                // Cells should link to all its placed objects, landscape and navmeshes
                // Placed should link to their cell and make sure it is retained when they are retained
                case ICellGetter cell:
                    var cellIdentifier = new FormLinkIdentifier(cell.ToFormLinkInformation());
                    graph.AddVertex(cellIdentifier);

                    // There is only one exterior cell which is persistent and contains all persistent records from all exterior cells
                    // Instead of adding the things placed in this cell, add them as if they are in the cells they are actually placed in
                    if (cell.IsExteriorCell() && cell.MajorFlags.HasFlag(Cell.MajorFlag.Persistent)) {
                        var worldspace = cell.GetWorldspace(editorEnvironment.LinkCache);
                        if (worldspace is null) break;

                        foreach (var placed in cell.Persistent) {
                            if (placed.Placement is not {} placement) continue;

                            var cellCoordinates = placement.GetCellCoordinates();
                            var actualCell = worldspace.GetCell(cellCoordinates);
                            if (actualCell is null) continue;

                            var actualCellIdentifier = new FormLinkIdentifier(actualCell.ToFormLinkInformation());
                            var placedIdentifier = new FormLinkIdentifier(placed.ToFormLinkInformation());
                            graph.AddEdge(new Edge<ILinkIdentifier>(actualCellIdentifier, placedIdentifier));
                            graph.AddEdge(new Edge<ILinkIdentifier>(placedIdentifier, actualCellIdentifier));
                        }
                        break;
                    }

                    foreach (var placed in cell.Temporary.Concat(cell.Persistent)) {
                        var placedIdentifier = new FormLinkIdentifier(placed.ToFormLinkInformation());
                        graph.AddEdge(new Edge<ILinkIdentifier>(cellIdentifier, placedIdentifier));
                        graph.AddEdge(new Edge<ILinkIdentifier>(placedIdentifier, cellIdentifier));
                    }

                    if (cell.Landscape is not null) {
                        var landscapeIdentifier = new FormLinkIdentifier(cell.Landscape.ToFormLinkInformation());
                        graph.AddEdge(new Edge<ILinkIdentifier>(cellIdentifier, landscapeIdentifier));
                    }

                    foreach (var navmesh in cell.NavigationMeshes) {
                        var navmeshIdentifier = new FormLinkIdentifier(navmesh.ToFormLinkInformation());
                        graph.AddEdge(new Edge<ILinkIdentifier>(cellIdentifier, navmeshIdentifier));
                    }
                    break;
                case IDialogTopicGetter topic:
                    var topicIdentifier = new FormLinkIdentifier(topic.ToFormLinkInformation());
                    graph.AddVertex(topicIdentifier);

                    foreach (var responses in topic.Responses) {
                        var responsesIdentifier = new FormLinkIdentifier(responses.ToFormLinkInformation());
                        graph.AddEdge(new Edge<ILinkIdentifier>(topicIdentifier, responsesIdentifier));
                    }
                    break;
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

                    // Remove these connections and fix them manually
                    // Removing references from location to something - potentially not needed (clearing crime factions list might have fixed it) / too broad
                    if (currentReference.Type == typeof(ILocationGetter)) {
                        if (current.Type != typeof(ILocationGetter)) continue;
                    }

                    // Is regenerated
                    if (currentReference.Type == typeof(INavigationMeshInfoMapGetter)) continue;

                    // Removing references from worldspaces to something like large refs or all recursive nodes from cells etc
                    if (currentReference.Type == typeof(IWorldspaceGetter)) continue;

                    // Navmesh to navmesh links will connect all cells in the worldspace which we don't want - re-finalize navmesh after cleaning!
                    if (current.Type == typeof(INavigationMeshGetter) && currentReference.Type == typeof(INavigationMeshGetter)) continue;

                    // Skip cells referencing everything their placed objects are referencing but we don't retain all of them
                    // TODO remove when the references don't count nested references anymore, needs Mutagen update
                    if (currentReference.Type == typeof(ICellGetter)) {
                        if (current.Type == typeof(INavigationMeshGetter)) continue;

                        if (current.Type != typeof(IEncounterZoneGetter)
                         && current.Type != typeof(IImageSpaceGetter)
                         && current.Type != typeof(IMusicTypeGetter)
                         && current.Type != typeof(IWaterGetter)
                         && current.Type != typeof(ILightingTemplateGetter)
                         && current.Type != typeof(IRegionGetter)) {
                            continue;
                        }
                    }

                    var currentReferenceLink = new FormLinkIdentifier(currentReference);
                    queue.Enqueue(currentReference);

                    graph.AddEdge(new Edge<ILinkIdentifier>(currentReferenceLink, new FormLinkIdentifier(current)));
                }

                foreach (var assetReference in referenceService.GetAssetReferences(current)) {
                    try {
                        var assetLink = assetTypeService.GetAssetLink(assetReference);
                        if (assetLink is null) continue;

                        var assetLinkIdentifier = new AssetLinkIdentifier(assetLink);
                        if (!graph.ContainsVertex(assetLinkIdentifier)) {
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

        foreach (var location in mod.EnumerateMajorRecords<ILocationGetter>()) {
            var locationLink = new FormLinkIdentifier(location.ToFormLinkInformation());
            if (location.Keywords is not null) {
                foreach (var keyword in location.Keywords) {
                    graph.AddEdge(new Edge<ILinkIdentifier>(locationLink, new FormLinkIdentifier(keyword)));
                }
            }
            if (!location.ParentLocation.IsNull) graph.AddEdge(new Edge<ILinkIdentifier>(locationLink, new FormLinkIdentifier(location.ParentLocation)));
            if (!location.Music.IsNull) graph.AddEdge(new Edge<ILinkIdentifier>(locationLink, new FormLinkIdentifier(location.Music)));
            if (!location.UnreportedCrimeFaction.IsNull) graph.AddEdge(new Edge<ILinkIdentifier>(locationLink, new FormLinkIdentifier(location.UnreportedCrimeFaction)));
            if (!location.WorldLocationMarkerRef.IsNull) graph.AddEdge(new Edge<ILinkIdentifier>(locationLink, new FormLinkIdentifier(location.WorldLocationMarkerRef)));
        }
    }

    public static HashSet<FormLinkInformation> GetRecordsToClean(
        HashSet<ILinkIdentifier> retainedLinks,
        IModGetter mod) {
        return mod.EnumerateMajorRecords()
            .Select(r => r.ToFormLinkInformation())
            .Except(retainedLinks
                .OfType<FormLinkIdentifier>()
                .Select(x => x.FormLink))
            .ToHashSet();
    }

    public void CreatedCleanedMod(ISkyrimModGetter mod, HashSet<FormLinkInformation> recordsToClean) {
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
        var newModKey = ModKey.FromFileName("Cleaned" + mod.ModKey.FileName);
        var newModPath = new ModPath(newModKey, fileSystem.Path.Combine(fileSystemRoot, newModKey.FileName.String));
        fileSystem.File.Move(oldModPath, newModPath);

        // Read renamed mod as new mod
        var duplicate = ModFactory.ImportSetter(newModPath,
            mod.GameRelease,
            BinaryReadParameters.Default with {
                FileSystem = fileSystem,
                LinkCache = editorEnvironment.LinkCache,
            });

        foreach (var record in recordsToClean) {
            if (record.Type != typeof(ICellGetter)) continue;
            if (!editorEnvironment.LinkCache.TryResolve<ICellGetter>(record.FormKey, out var cell)) continue;

            // For exterior cells we cannot remove the persistent placed records by removing the cells
            // There they are stored in the global persistent cell per worldspace 
            var placedToRemove = cell.IsInteriorCell() ? cell.Temporary.Concat(cell.Persistent) : cell.Temporary;
            foreach (var placed in placedToRemove) {
                recordsToClean.Remove(placed.ToFormLinkInformation());
            }

            if (cell.Landscape is not null) {
                recordsToClean.Remove(cell.Landscape.ToFormLinkInformation());
            }

            foreach (var navmesh in cell.NavigationMeshes) {
                recordsToClean.Remove(navmesh.ToFormLinkInformation());
            }
        }

        using var countingNotifier = new CountingNotifier(notificationService, $"Cleaning Records from {mod.ModKey}", recordsToClean.Count);
        foreach (var record in recordsToClean) {
            countingNotifier.NextStep();
            duplicate.Remove(new FormKey(duplicate.ModKey, record.FormKey.ID), record.Type);
        }
        countingNotifier.Stop();

        editorEnvironment.Update(updater => updater
            .LoadOrder.AddMutableMods(duplicate)
            .Build());
    }

    public void CreatedCleanerOverrideMod(ISkyrimModGetter mod, IEnumerable<IFormLinkIdentifier> recordsToClean) {
        var cleanMod = editorEnvironment.AddNewMutableMod(ModKey.FromFileName($"Clean{mod.ModKey.Name}.esp"));

        foreach (var record in recordsToClean) {
            var recordOverride = recordController.GetOrAddOverride(record, cleanMod);
            recordOverride.IsDeleted = true;
        }
    }

    private static readonly Type[] SelfRetainedRecordTypes = [
        typeof(IIdleAnimationGetter),
        typeof(IAddonNodeGetter),
        typeof(IAnimatedObjectGetter),
    ];

    public void RetainLinks(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies,
        FormLinkIdentifier formLinkIdentifier,
        HashSet<ILinkIdentifier> retained,
        IReadOnlySet<ILinkIdentifier> excluded,
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> dependencyGraph,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        var formLink = formLinkIdentifier.FormLink;
        if (formLink.FormKey.ModKey != mod.ModKey
         || essentialRecordProvider.EssentialRecords.Contains(formLink)
         || (dependencies.Count > 0 && editorEnvironment.LinkCache.ResolveAllSimpleContexts(formLink).Any(c => dependencies.Contains(c.ModKey)))) {
            // Retain overrides of records from other mods
            // Retain records that are essential and all their transitive dependencies
            // Retain placeholder records that are going to be replaced by Creation Club records via patch
            // Retain things that are overridden by dependencies
            retained.Add(formLinkIdentifier);
            dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(formLinkIdentifier, formLinkIdentifier));

            if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) return;

            retainOutgoingEdges(edges);
        } else if (formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)) {
            // Retain records that are self-retained, and keep adding all other records that are linked to them
            var queue = new Queue<ILinkIdentifier>([formLinkIdentifier]);
            dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(formLinkIdentifier, formLinkIdentifier));
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(formLinkIdentifier, current));

                if (!retained.Add(current)) continue;
                if (excluded.Contains(current)) continue;
                if (!graph.IncomingEdges.TryGetValue(current, out var currentEdges)) continue;

                queue.Enqueue(currentEdges.Select(x => x.Target)
                    .Where(t => formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)));
            }
        }
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

    public void FinalRetainLinks(
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        HashSet<ILinkIdentifier> retained,
        IReadOnlySet<ILinkIdentifier> excludedLinks,
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> dependencyGraph,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        RetainCellsAroundRegion(retained, graph, retainOutgoingEdges);

        // Retain records that link to any records that are retained
        // These records don't retain any other records implicitly in the current selection
        foreach (var implicitType in ImplicitRetainedRecordTypes) {
            foreach (var vertex in graph.Vertices) {
                if (excludedLinks.Contains(vertex)) continue;
                if (vertex is not FormLinkIdentifier formLinkIdentifier) continue;
                if (formLinkIdentifier.FormLink.Type != implicitType) continue;
                if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;

                // Don't retain links to parent or previous nodes
                if (formLinkIdentifier.FormLink.Type == typeof(IStoryManagerQuestNodeGetter)) {
                    edges = edges
                        .Where(x => x.Target is FormLinkIdentifier f
                         && f.FormLink.Type != typeof(IStoryManagerQuestNodeGetter)
                         && f.FormLink.Type != typeof(IStoryManagerBranchNodeGetter))
                        .ToHashSet();
                }

                if (edges.Count == 0) continue;

                // If all referenced records are retained, retain this too
                if (formLinkIdentifier.FormLink.Type == typeof(IConstructibleObjectGetter) || formLinkIdentifier.FormLink.Type == typeof(IRelationshipGetter)) {
                    if (edges.Any(x => !retained.Contains(x.Target))) continue;
                } else {
                    if (edges.Any(x => !retained.Contains(x.Target))
                     && edges.All(x => !retained.Contains(x.Target))) continue;
                }

                // Keep parent nodes of quest nodes
                if (formLinkIdentifier.FormLink.Type == typeof(IStoryManagerQuestNodeGetter)
                 && editorEnvironment.LinkCache.TryResolve<IStoryManagerQuestNodeGetter>(formLinkIdentifier.FormLink.FormKey, out var questNode)) {
                    var retainedQuests = questNode.Quests
                        .Select(x => x.Quest.ToStandardizedIdentifier())
                        .Where(x => retained.Contains(new FormLinkIdentifier(x)))
                        .ToArray();

                    if (retainedQuests.Length == 0) continue;

                    edges = edges
                        .Where(x => x.Target is not FormLinkIdentifier f
                         || f.FormLink.Type != typeof(IQuestGetter)
                         || retainedQuests.Contains(f.FormLink.ToStandardizedIdentifier()))
                        .ToHashSet();

                    var parentNode = questNode.Parent.TryResolve(editorEnvironment.LinkCache);
                    while (parentNode is not null) {
                        var link = new FormLinkIdentifier(parentNode.ToFormLinkInformation());
                        if (excludedLinks.Contains(link)) break;

                        var edge = new Edge<ILinkIdentifier>(vertex, link);
                        dependencyGraph.AddEdge(edge);
                        retained.Add(link);
                        retainOutgoingEdges([edge]);
                        parentNode = parentNode.Parent.TryResolve(editorEnvironment.LinkCache);
                    }
                }

                retained.Add(vertex);

                retainOutgoingEdges(edges);
            }
        }

        foreach (var vertex in graph.Vertices) {
            if (excludedLinks.Contains(vertex)) continue;
            if (vertex is not FormLinkIdentifier { FormLink: var formLink } formLinkIdentifier) continue;

            if (formLink.Type == typeof(ISceneGetter)) {
                // Retain scenes that begin on quest start                                                                       
                var scene = editorEnvironment.LinkCache.Resolve<ISceneGetter>(formLink.FormKey);
                if (scene.Flags is null || !scene.Flags.Value.HasFlag(Scene.Flag.BeginOnQuestStart)) continue;
                if (!retained.Contains(new FormLinkIdentifier(scene.Quest))) continue;

                retained.Add(formLinkIdentifier);

                if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) continue;

                retainOutgoingEdges(edges);
            } else if (formLink.Type == typeof(IDialogTopicGetter)) {
                // Only scene dialog topics can be unused, everything else is implicitly retained
                try {
                    var topic = editorEnvironment.LinkCache.Resolve<IDialogTopicGetter>(formLink.FormKey);
                    if (topic.SubtypeName.ToDialogTopicSubtype() != DialogTopic.SubtypeEnum.Scene) {
                        if (!retained.Contains(new FormLinkIdentifier(topic.Quest))) continue;

                        retained.Add(formLinkIdentifier);

                        if (!graph.OutgoingEdges.TryGetValue(formLinkIdentifier, out var edges)) continue;

                        retainOutgoingEdges(edges);
                    }
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
    }

    public void RetainCellsAroundRegion(
        HashSet<ILinkIdentifier> retained,
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> dependencyGraph,
        Action<HashSet<Edge<ILinkIdentifier>>> retainOutgoingEdges) {
        foreach (var (worldspaceFormKey, retainedCells) in featureFlagService.EnabledFeatureFlags.EnumerateRetainedCells(editorEnvironment.LinkCache)) {
            if (!editorEnvironment.LinkCache.TryResolve<IWorldspaceGetter>(worldspaceFormKey, out var worldspace)) continue;

            var retainedCoordinates = retainedCells
                .Select(x => x.Grid?.Point)
                .WhereNotNull()
                .ToHashSet();

            foreach (var retainedCell in retainedCells) {
                if (retainedCell.Grid is null) continue;

                var retainedCoordinate = retainedCell.Grid.Point;
                var sourceLink = new FormLinkIdentifier(retainedCell.ToFormLinkInformation());

                for (var dx = -3; dx <= 3; dx++) {
                    for (var dy = -3; dy <= 3; dy++) {
                        var position = new P2Int(retainedCoordinate.X + dx, retainedCoordinate.Y + dy);
                        if (retainedCoordinates.Contains(position)) continue;

                        var cell = worldspace.GetCell(position);
                        if (cell is null) continue;

                        // Skip cells referencing everything their placed objects are referencing but we don't retain all of them
                        var cellLink = new FormLinkIdentifier(cell.ToFormLinkInformation());
                        dependencyGraph.AddEdge(new Edge<ILinkIdentifier>(sourceLink, cellLink));
                        retainOutgoingEdges([
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Location)),
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Owner)),
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.AcousticSpace)),
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.EncounterZone)),
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.ImageSpace)),
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Music)),
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Water)),
                            new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.LightingTemplate)),
                            ..cell.Regions is null ? [] : cell.Regions.Select(r => new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(r))),
                        ]);
                        retained.Add(cellLink);

                        foreach (var placed in cell.Temporary.Concat(cell.Persistent)) {
                            if (placed is not IPlacedObjectGetter placedObject) continue;

                            // Skip owned stuff because that would retain npcs/factions we don't want to retain necessarily
                            if (!placed.Owner.IsNull && !retained.Contains(new FormLinkIdentifier(placed.Owner))) continue;

                            // Skip stuff with scripts because they might reference anything
                            if (placed.VirtualMachineAdapter is not null) continue;

                            var placeableObject = placedObject.Base.TryResolve(editorEnvironment.LinkCache);
                            if (placeableObject is IFloraGetter or IFurnitureGetter or IStaticGetter or IMoveableStaticGetter or ITreeGetter) {
                                // Exclude markers, we just care about big things that are visible
                                if (placeableObject.EditorID is not null && placeableObject.EditorID.Contains("Marker")) continue;

                                Retain(placed);
                            }
                        }

                        if (cell.Landscape is not null) {
                            Retain(cell.Landscape);
                        }
                    }
                }

                void Retain(IMajorRecordGetter record) {
                    var link = new FormLinkIdentifier(record.ToFormLinkInformation());
                    var edge = new Edge<ILinkIdentifier>(sourceLink, link);
                    dependencyGraph.AddEdge(edge);
                    retainOutgoingEdges([edge]);
                    retained.Add(link);
                }
            }
        }
    }
}
