using System.Collections.Concurrent;
using System.IO.Abstractions;
using BuildStripper.Models;
using BuildStripper.Models.FeatureFlag;
using CreationEditor;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Serilog;
using ILinkIdentifier = BuildStripper.Models.ILinkIdentifier;
namespace BuildStripper.Services;

public sealed class RecordCleaner(
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    ILogger logger,
    IFileSystem fileSystem,
    IRecordController recordController,
    IAssetTypeService assetTypeService,
    IReferenceService referenceService) {

    /// <summary>
    /// Adds all references to records to the reference graph.
    /// </summary>
    /// <param name="graph">Reference graph to add references to</param>
    /// <param name="mod">Mod to get references for</param>
    /// <param name="dependencies">List of mods that are dependent on the mod in question and are relevant for the reference graph</param>
    /// <param name="masters">List of masters of the mod</param>
    public void BuildGraph(Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph, IModGetter mod, IReadOnlyList<ModKey> dependencies, IReadOnlyList<ModKey> masters) {
        var processedRecords = new ConcurrentDictionary<FormKey, bool>();
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
                // Force add links from music tracks to their music files with all possible extensions, as the game doesn't actually care which extension is used
                case IMusicTrackGetter musicTrack:
                    var musicTrackIdentifier = new FormLinkIdentifier(musicTrack.ToFormLinkInformation());
                    foreach (var musicFileExtension in assetTypeService.Provider.Music.FileExtensions) {
                        var musicFilePathWithOtherExtension = fileSystem.Path.ChangeExtension(musicTrack.TrackFilename, musicFileExtension);
                        if (musicFilePathWithOtherExtension is null) continue;

                        var assetLink = assetTypeService.GetAssetLink(musicFilePathWithOtherExtension);
                        if (assetLink is null) continue;

                        var assetLinkIdentifier = new AssetLinkIdentifier(assetLink);
                        graph.AddEdge(new Edge<ILinkIdentifier>(musicTrackIdentifier, assetLinkIdentifier));
                    }
                    break;
            }

            // Add all transitive dependencies of the record
            var queue = new Queue<IFormLinkIdentifier>([record.ToFormLinkInformation()]);
            while (queue.Count > 0) {
                var current = queue.Dequeue();
                if (!processedRecords.TryAdd(current.FormKey, true)) {
                    continue;
                }

                graph.AddVertex(new FormLinkIdentifier(current));

                foreach (var currentReference in referenceService.GetRecordReferences(current)) {
                    // This just checks if the reference was defined in the mod or one of its dependencies. Update if needed.
                    var modKey = currentReference.FormKey.ModKey;
                    if (modKey != mod.ModKey
                     && !masters.Contains(modKey)
                     && !dependencies.Contains(modKey)) continue;

                    // Remove auto generated entries from locations, and only retain custom referenced record
                    if (currentReference.Type == typeof(ILocationGetter)
                     && editorEnvironment.LinkCache.TryResolve<ILocationGetter>(currentReference.FormKey, out var location)) {
                        if (current.FormKey != location.ParentLocation.FormKey
                         && current.FormKey != location.Music.FormKey
                         && current.FormKey != location.UnreportedCrimeFaction.FormKey
                         && current.FormKey != location.HorseMarkerRef.FormKey
                         && current.FormKey != location.WorldLocationMarkerRef.FormKey
                         && (location.Keywords is null || location.Keywords.All(k => k.FormKey != current.FormKey))) {
                            continue;
                        }
                    }

                    // NAVI is going to be regenerated anyway, don't include that
                    if (currentReference.Type == typeof(INavigationMeshInfoMapGetter)) continue;

                    // Removing references from worldspaces to something like large refs or all recursive nodes from cells etc
                    if (currentReference.Type == typeof(IWorldspaceGetter)) continue;

                    // Navmesh to navmesh links will connect all cells in the worldspace which we don't want - re-finalize navmesh after cleaning!
                    if (currentReference.Type == typeof(INavigationMeshGetter) && current.Type == typeof(INavigationMeshGetter)) continue;

                    // Don't retain links previous story manager nodes - this can be regenerated later on and has no semantic meaning
                    if ((currentReference.Type == typeof(IStoryManagerQuestNodeGetter) || currentReference.Type == typeof(IStoryManagerBranchNodeGetter))
                     && editorEnvironment.LinkCache.TryResolve<IAStoryManagerNodeGetter>(currentReference.FormKey, out var storyManagerNode)
                     && current.FormKey == storyManagerNode.PreviousSibling.FormKey) {
                        continue;
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

    public void CreatedCleanedMod(
        ISkyrimModGetter mod,
        HashSet<FormLinkInformation> recordsToClean,
        IReadOnlyDictionary<IFormLinkIdentifier, Action<IMajorRecord>> postProcessSteps) {
        var cleanedModKey = ModKey.FromFileName("Cleaned" + mod.ModKey.FileName);
        var duplicate = mod.Duplicate(cleanedModKey);

        var translatedRecordsToClean = recordsToClean.Select(x => new FormLinkInformation(new FormKey(duplicate.ModKey, x.FormKey.ID), x.Type)).ToHashSet();
        duplicate.Remove(translatedRecordsToClean);
        var linkCache = duplicate.ToUntypedMutableLinkCache();

        foreach (var (formLinkIdentifier, postProcessStep) in postProcessSteps) {
            var translatedFormKey = new FormKey(duplicate.ModKey, formLinkIdentifier.FormKey.ID);
            if (linkCache.TryResolve(translatedFormKey, formLinkIdentifier.Type, out var record)) {
                if (record is IMajorRecord recordSetter) {
                    postProcessStep(recordSetter);
                }
            }
        }

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
        typeof(IAddonNodeGetter),
        typeof(IAnimatedObjectGetter),
    ];

    /// <summary>
    /// Adds link to the retained graph if the given form link is:
    /// - an override of a record from another mod
    /// - configured as a record that is essential for the mod
    /// - being overridden by a dependency of the mod
    /// - a record that is always retained due to lacking tracking capabilities currently (like an animated object or addon node)
    /// </summary>
    /// <param name="essentialRecordProvider">Provider for essential records</param>
    /// <param name="retainedGraph">Filtered graph of all links that are retained in the mod and its dependencies</param>
    /// <param name="mod">Mod to find retained records for</param>
    /// <param name="dependencies">List of mods that are dependent on the mod, any links to the mod in the dependencies will be retained</param>
    /// <param name="formLinkIdentifier">Form link identifier to check for retention</param>
    public void RetainLinks(
        IEssentialRecordProvider essentialRecordProvider,
        FilteredGraph<ILinkIdentifier, Edge<ILinkIdentifier>> retainedGraph,
        IModGetter mod,
        IReadOnlyList<ModKey> dependencies,
        FormLinkIdentifier formLinkIdentifier) {
        var formLink = formLinkIdentifier.FormLink;
        if (formLink.FormKey.ModKey != mod.ModKey
         || essentialRecordProvider.IsEssentialRecord(mod.ModKey, formLink)
         || (dependencies.Count > 0 && editorEnvironment.LinkCache.ResolveAllSimpleContexts(formLink).Any(c => dependencies.Contains(c.ModKey)))) {
            // Retain overrides of records from other mods
            // Retain records that are essential and all their transitive dependencies
            // Retain things that are overridden by dependencies
            if (retainedGraph.ExcludedVertices.Contains(formLinkIdentifier)) return;

            retainedGraph.IncludeVertex(formLinkIdentifier, formLinkIdentifier);
        } else if (formLink.Type.InheritsFromAny(SelfRetainedRecordTypes)) {
            // Retain all records that are self-retained
            if (retainedGraph.ExcludedVertices.Contains(formLinkIdentifier)) return;

            retainedGraph.IncludeVertex(formLinkIdentifier, formLinkIdentifier);
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

    /// <summary>
    /// After the initial retention of records, this method will retain any records based on the existing retained records.
    /// This includes:
    /// - retaining cells around retained regions, with different retention rules based on the distance from the retained region
    /// - implicitly retained records that are only retained if they are linked to by any retained records
    /// - 
    /// </summary>
    /// <param name="mod"></param>
    /// <param name="essentialRecordProvider"></param>
    /// <param name="graph"></param>
    /// <param name="retainedGraph"></param>
    /// <param name="addPostProcessStep"></param>
    public void FinalRetainLinks(
        IModGetter mod,
        IEssentialRecordProvider essentialRecordProvider,
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        FilteredGraph<ILinkIdentifier, Edge<ILinkIdentifier>> retainedGraph,
        Action<IFormLinkIdentifier, Action<IMajorRecord>> addPostProcessStep) {
        RetainCellsAroundRegion(mod, essentialRecordProvider, graph, retainedGraph, addPostProcessStep);

        // Retain records that link to any records that are retained
        // These records don't retain any other records implicitly in the current selection
        foreach (var vertex in graph.Vertices) {
            if (retainedGraph.ExcludedVertices.Contains(vertex)) continue;
            if (vertex is not FormLinkIdentifier formLinkIdentifier) continue;
            if (!ImplicitRetainedRecordTypes.Contains(formLinkIdentifier.FormLink.Type)) continue;
            if (!graph.OutgoingEdges.TryGetValue(vertex, out var edges)) continue;

            // Don't retain these if based on having parent or previous nodes that are retained - so filter them out for this check
            if (formLinkIdentifier.FormLink.Type == typeof(IStoryManagerQuestNodeGetter) || formLinkIdentifier.FormLink.Type == typeof(IStoryManagerBranchNodeGetter)) {
                edges = edges
                    .Where(x => x.Target is FormLinkIdentifier f
                     && f.FormLink.Type != typeof(IStoryManagerQuestNodeGetter)
                     && f.FormLink.Type != typeof(IStoryManagerBranchNodeGetter))
                    .ToHashSet();
            }

            if (edges.Count == 0) continue;

            var builtFilteredGraph = retainedGraph.Build();

            if (formLinkIdentifier.FormLink.Type == typeof(IConstructibleObjectGetter) || formLinkIdentifier.FormLink.Type == typeof(IRelationshipGetter)) {
                // Constructible objects and relationships should only be retained if all their references are retained
                if (edges.Any(x => !builtFilteredGraph.ContainsVertex(x.Target))) continue;
            } else {
                // Other types should be retained if any of their references are retained
                if (!edges.Any(x => builtFilteredGraph.ContainsVertex(x.Target))) continue;
            }

            // Keep parent nodes of quest nodes
            if (formLinkIdentifier.FormLink.Type == typeof(IStoryManagerQuestNodeGetter)
             && editorEnvironment.LinkCache.TryResolve<IStoryManagerQuestNodeGetter>(formLinkIdentifier.FormLink.FormKey, out var questNode)) {
                // Only keep quest edges for quests that are retained
                var retainedQuests = questNode.Quests
                    .Select(x => x.Quest.ToStandardizedIdentifier())
                    .Where(x => builtFilteredGraph.ContainsVertex(new FormLinkIdentifier(x)))
                    .ToArray();

                if (retainedQuests.Length == 0) continue;
            }

            retainedGraph.IncludeVertex(vertex, vertex);
        }

        foreach (var vertex in graph.Vertices) {
            if (retainedGraph.ExcludedVertices.Contains(vertex)) continue;
            if (vertex is not FormLinkIdentifier { FormLink: var formLink } formLinkIdentifier) continue;

            if (formLink.Type == typeof(ISceneGetter)) {
                // Retain scenes that begin on quest start                                                                       
                if (!editorEnvironment.LinkCache.TryResolve<ISceneGetter>(formLink.FormKey, out var scene)) {
                    logger.Here().Warning("Failed to resolve scene {Scene}", formLink.FormKey);
                    continue;
                }

                if (scene.Flags is null || !scene.Flags.Value.HasFlag(Scene.Flag.BeginOnQuestStart)) continue;

                retainedGraph.IncludeVertex(formLinkIdentifier, new FormLinkIdentifier(scene.Quest));
            } else if (formLink.Type == typeof(IDialogTopicGetter)) {
                // Only scene dialog topics can be unused, everything else is implicitly retained by the quest
                if (!editorEnvironment.LinkCache.TryResolve<IDialogTopicGetter>(formLink.FormKey, out var topic)) {
                    logger.Here().Warning("Failed to resolve dialog topic {Topic}", formLink.FormKey);
                    continue;
                }

                if (topic.SubtypeName.ToDialogTopicSubtype() != DialogTopic.SubtypeEnum.Scene) {
                    retainedGraph.IncludeVertex(formLinkIdentifier, new FormLinkIdentifier(topic.Quest));
                }
            }
        }
    }

    private void RetainCellsAroundRegion(
        IModGetter mod,
        IEssentialRecordProvider essentialRecordProvider,
        Graph<ILinkIdentifier, Edge<ILinkIdentifier>> graph,
        FilteredGraph<ILinkIdentifier, Edge<ILinkIdentifier>> retainedGraph,
        Action<IFormLinkIdentifier, Action<IMajorRecord>> addPostProcessStep) {
        foreach (var (worldspaceFormKey, retainedCells) in essentialRecordProvider.EnumerateRetainedExteriorCells(mod.ModKey, editorEnvironment.LinkCache)) {
            if (!editorEnvironment.LinkCache.TryResolve<IWorldspaceGetter>(worldspaceFormKey, out var worldspace)) continue;

            foreach (var (cell, retainReason) in retainedCells) {
                if (cell.Grid is null) continue;

                var position = cell.Grid.Point;
                var sourceCell = new FormLinkIdentifier(cell.ToFormLinkInformation());

                // Skip cells referencing everything their placed objects are referencing, but we don't retain all of them
                var cellLink = new FormLinkIdentifier(cell.ToFormLinkInformation());
                if (retainedGraph.ExcludedVertices.Contains(cellLink)) continue;

                switch (retainReason) {
                    case ExteriorCellRetainReason.WithinLandscapeRangeOfRetainedCell: {
                        // If the cell is just outside the playable area, we want to retain the landscape shape but nothing else
                        // This is done so we can ensure that players who have region borders disabled don't crash directly when loading a cell
                        // that is outside the playable area, and they know when they are getting out of bounds because they will see only brown landscape
                        // To implement this, use a post-processing step to clear out all cell contents apart from the landscape shape
                        addPostProcessStep(cell.ToFormLinkInformation(), EmptyCell);

                        // Force exclude everything placed in the cell
                        foreach (var placed in worldspace.GetAllPlacedInExteriorCell(position)) {
                            var formLinkIdentifier = new FormLinkIdentifier(placed.ToFormLinkInformation());
                            retainedGraph.ExcludeVertex(formLinkIdentifier);
                        }

                        // Remove links from the cell to anything that we remove as part of the empty cell generation
                        // to make sure they are not retained due to this cell being retained
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Location)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Owner)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.LockList)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.AcousticSpace)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.EncounterZone)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.ImageSpace)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Music)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Water)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.LightingTemplate)));

                        if (cell.Landscape is not null) {
                            if (cell.Landscape.Textures is not null) {
                                foreach (var texture in cell.Landscape.Textures) {
                                    graph.RemoveEdge(new Edge<ILinkIdentifier>(new FormLinkIdentifier(cell.Landscape), new FormLinkIdentifier(texture)));
                                }
                            }

                            foreach (var layer in cell.Landscape.Layers) {
                                if (layer.Header is null) continue;

                                graph.RemoveEdge(new Edge<ILinkIdentifier>(new FormLinkIdentifier(cell.Landscape), new FormLinkIdentifier(layer.Header.Texture)));
                            }
                        }

                        foreach (var navigationMesh in cell.NavigationMeshes) {
                            graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(navigationMesh.ToFormLinkInformation())));
                        }

                        foreach (var temporary in cell.Temporary) {
                            graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(temporary)));
                        }

                        break;

                        void EmptyCell(IMajorRecord record) {
                            if (record is not ICell c) return;

                            c.EditorID = null;
                            c.Location.SetToNull();
                            c.Owner.SetToNull();
                            c.LockList.SetToNull();
                            c.AcousticSpace.SetToNull();
                            c.EncounterZone.SetToNull();
                            c.ImageSpace.SetToNull();
                            c.Music.SetToNull();
                            c.Water.SetToNull();
                            c.LightingTemplate.SetToNull();
                            c.Landscape?.Textures?.Clear();
                            c.Landscape?.Layers.Clear();
                            c.NavigationMeshes.Clear();
                            c.Temporary.Clear();
                            c.Persistent.Clear();

                            // TODO also remove persistent records from the top level cell
                        }
                    }
                    case ExteriorCellRetainReason.WithinViewDistanceOfRetainedCell: {
                        // Retain the cell and all its references
                        Retain(new FormLinkIdentifier(cell.Location));
                        Retain(new FormLinkIdentifier(cell.Owner));
                        Retain(new FormLinkIdentifier(cell.LockList));
                        Retain(new FormLinkIdentifier(cell.AcousticSpace));
                        Retain(new FormLinkIdentifier(cell.EncounterZone));
                        Retain(new FormLinkIdentifier(cell.ImageSpace));
                        Retain(new FormLinkIdentifier(cell.Music));
                        Retain(new FormLinkIdentifier(cell.Water));
                        Retain(new FormLinkIdentifier(cell.LightingTemplate));
                        if (cell.Regions is not null) {
                            foreach (var region in cell.Regions) {
                                retainedGraph.IncludeVertex(new FormLinkIdentifier(region), sourceCell);
                            }
                        }

                        // Include only relevant visible placed objects
                        foreach (var placed in worldspace.GetAllPlacedInExteriorCell(position)) {
                            if (ShouldBeRetainedOutsidePlayableAreaWithinUGridsToLoad(placed, retainedGraph, editorEnvironment)) {
                                RetainRecord(placed);
                            } else {
                                // If the placed shouldn't be retained, we want to make sure it actually stays excluded
                                var formLinkIdentifier = new FormLinkIdentifier(placed.ToFormLinkInformation());
                                retainedGraph.ExcludeVertex(formLinkIdentifier);
                            }
                        }
                        
                        addPostProcessStep(cell.ToFormLinkInformation(), EmptyCell);

                        // Remove links from the cell to anything that we remove as part of the empty cell generation
                        // to make sure they are not retained due to this cell being retained
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Location)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Owner)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.LockList)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.AcousticSpace)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.EncounterZone)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.ImageSpace)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Music)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.Water)));
                        graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(cell.LightingTemplate)));

                        foreach (var navigationMesh in cell.NavigationMeshes) {
                            graph.RemoveEdge(new Edge<ILinkIdentifier>(cellLink, new FormLinkIdentifier(navigationMesh.ToFormLinkInformation())));
                        }

                        break;

                        void EmptyCell(IMajorRecord record) {
                            if (record is not ICell c) return;

                            c.EditorID = null;
                            c.Location.SetToNull();
                            c.Owner.SetToNull();
                            c.LockList.SetToNull();
                            c.AcousticSpace.SetToNull();
                            c.EncounterZone.SetToNull();
                            c.ImageSpace.SetToNull();
                            c.Music.SetToNull();
                            c.Water.SetToNull();
                            c.LightingTemplate.SetToNull();
                            c.NavigationMeshes.Clear();
                        }
                    }
                }

                // Always include the cell itself and its landscape
                retainedGraph.IncludeVertex(cellLink, sourceCell);
                if (cell.Landscape is not null) {
                    RetainRecord(cell.Landscape);
                }

                // Exclude navmeshes outside the playable area
                foreach (var navigationMesh in cell.NavigationMeshes) {
                    var formLinkIdentifier = new FormLinkIdentifier(navigationMesh.ToFormLinkInformation());
                    retainedGraph.ExcludeVertex(formLinkIdentifier);
                }

                void RetainRecord(IMajorRecordGetter record) {
                    var link = new FormLinkIdentifier(record.ToFormLinkInformation());
                    Retain(link);
                }
                void Retain(FormLinkIdentifier link) {
                    if (retainedGraph.ExcludedVertices.Contains(link)) return;

                    retainedGraph.IncludeVertex(link, sourceCell);
                }
            }
        }
    }

    private static bool ShouldBeRetainedOutsidePlayableAreaWithinUGridsToLoad(
        IPlacedGetter placed,
        FilteredGraph<ILinkIdentifier, Edge<ILinkIdentifier>> retainedGraph,
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment) {
        if (placed is not IPlacedObjectGetter placedObject) return false;

        // Skip owned stuff because that would retain npcs/factions we don't want to retain necessarily
        if (!placed.Owner.IsNull && !retainedGraph.IncludedVertices.Contains(new FormLinkIdentifier(placed.Owner))) return false;

        // Skip stuff with scripts because they might reference anything
        if (placed.VirtualMachineAdapter is not null) return false;

        var placeableObject = placedObject.Base.TryResolve(editorEnvironment.LinkCache);
        if (placeableObject is IFloraGetter or IFurnitureGetter or IStaticGetter or IMoveableStaticGetter or ITreeGetter) {
            // Exclude markers, we just care about big things that are visible
            return placeableObject.EditorID is null || !placeableObject.EditorID.Contains("Marker");
        }

        return false;
    }
}
