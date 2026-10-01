using BuildStripper.Models.FeatureFlag;
using BuildStripper.Services.FeatureFlag;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace BuildStripper.Services;

public sealed class EssentialRecordProvider(
    IEditorEnvironment editorEnvironment,
    IFeatureFlagService featureFlagService) : IEssentialRecordProvider {

    public bool IsEssentialRecord(ModKey mod, IFormLinkGetter formLink) {
        if (!EssentialRecords.TryGetValue(mod, out var essentialRecords)) {
            essentialRecords = EnumerateEssentialRecords(mod)
                .Select(x => x.FormKey)
                .ToHashSet();

            EssentialRecords[mod] = essentialRecords;
        }

        return essentialRecords.Contains(formLink.FormKey);
    }

    private Dictionary<ModKey, IReadOnlySet<FormKey>> EssentialRecords { get; } = new();

    private IEnumerable<FormLinkInformation> EnumerateEssentialRecords(ModKey mod) {
        foreach (var (worldspace, cells) in EnumerateEssentialExteriorCells(mod, editorEnvironment.LinkCache)) {
            yield return new FormLinkInformation(worldspace, typeof(IWorldspaceGetter));

            foreach (var cell in cells) {
                yield return cell.ToFormLinkInformation();
            }
        }

        foreach (var featureFlag in featureFlagService.EnabledFeatureFlags.Where(f => f.ModKey == mod)) {
            foreach (var essentialRecord in featureFlag.EssentialRecords) {
                yield return essentialRecord;
            }
        }
    }

    public Dictionary<FormKey, List<(ICellGetter Cell, ExteriorCellRetainReason RetainReason)>> EnumerateRetainedExteriorCells(ModKey mod, ILinkCache linkCache) {
        var mergedFeatureFlags = EnumerateWorldspaceRegions(mod)
            .GroupBy(x => x.Worldspace);

        var retainedCells = new Dictionary<FormKey, List<(ICellGetter Cell, ExteriorCellRetainReason RetainReason)>>();
        foreach (var group in mergedFeatureFlags) {
            if (!group.Key.TryResolve(linkCache, out var worldspace)) continue;

            var regions = group.SelectMany(y => y.Regions).ToHashSet();

            var cellsWithinRange = new List<(ICellGetter Cell, ExteriorCellRetainReason RetainReason)>();
            retainedCells[worldspace.FormKey] = cellsWithinRange;

            if (regions.Count == 0) {
                foreach (var cell in worldspace.EnumerateCells()) {
                    cellsWithinRange.Add((cell, ExteriorCellRetainReason.WithinRegionBorder));
                }
                continue;
            }

            var cells = worldspace
                .EnumerateCells()
                .Where(c => c.Regions is not null && c.Regions.Intersect(regions).Any())
                .ToArray();

            var retainedCellsInWorldspace = cells.ToArray();

            var retainedCoordinates = retainedCellsInWorldspace
                .Select(x => x.Grid?.Point)
                .WhereNotNull()
                .ToHashSet();

            var cellViewDistanceRangeToKeep = group.Max(x => x.CellViewDistanceRangeToKeepOutsidePlayableArea);

            // Get all coordinates for cells within the landscape range of retained cells
            var processedCoordinates = new HashSet<P2Int>();
            foreach (var retainedCell in retainedCellsInWorldspace) {
                cellsWithinRange.Add((retainedCell, ExteriorCellRetainReason.WithinRegionBorder));

                if (retainedCell.Grid is null) continue;

                var retainedCoordinate = retainedCell.Grid.Point;
                processedCoordinates.Add(retainedCoordinate);

                // With a default uGridsToLoad = 5 diameter, the radius is 2
                for (var dx = -cellViewDistanceRangeToKeep; dx <= cellViewDistanceRangeToKeep; dx++) {
                    for (var dy = -cellViewDistanceRangeToKeep; dy <= cellViewDistanceRangeToKeep; dy++) {
                        processedCoordinates.Add(new P2Int(retainedCoordinate.X + dx, retainedCoordinate.Y + dy));
                    }
                }
            }

            // Add cells to the list based on their distance to retained cells
            foreach (var coordinate in processedCoordinates) {
                if (retainedCoordinates.Contains(coordinate)) continue;

                var minDistanceToRetainedCoordinates = retainedCoordinates
                    .Select(c => {
                        // We need to find the maximum distance to either direction to see if we're still inside the uGridsToLoad range
                        var distX = Math.Abs(c.X - coordinate.X);
                        var distY = Math.Abs(c.Y - coordinate.Y);
                        return Math.Max(distX, distY);
                    })
                    .Min();

                var cell = worldspace.GetCell(coordinate);
                if (cell is null) continue;

                if (minDistanceToRetainedCoordinates > 0 && minDistanceToRetainedCoordinates <= cellViewDistanceRangeToKeep) {
                    cellsWithinRange.Add((cell, ExteriorCellRetainReason.WithinViewDistanceOfRetainedCell));
                }
            }
        }

        return retainedCells;
    }

    public Dictionary<FormKey, List<ICellGetter>> EnumerateEssentialExteriorCells(ModKey mod, ILinkCache linkCache) {
        var mergedFeatureFlags = EnumerateWorldspaceRegions(mod)
            .GroupBy(x => x.Worldspace);

        var retainedCells = new Dictionary<FormKey, List<ICellGetter>>();
        foreach (var group in mergedFeatureFlags) {
            if (!group.Key.TryResolve(linkCache, out var worldspace)) continue;

            var regions = group.SelectMany(y => y.Regions).ToHashSet();
            var cells = regions.Count == 0
                ? worldspace.EnumerateCells()
                : worldspace.EnumerateCells().Where(c => c.Regions is not null && c.Regions.Intersect(regions).Any());

            foreach (var cell in cells) {
                retainedCells.GetOrAdd(worldspace.FormKey).Add(cell);
            }
        }

        return retainedCells;
    }

    public IEnumerable<WorldspaceRegions> EnumerateWorldspaceRegions(ModKey mod) {
        return featureFlagService.EnabledFeatureFlags
            .Where(f => f.ModKey == mod)
            .SelectMany(x => x.AllowedRegions);
    }

    public IEnumerable<WorldspaceRegions> EnumerateWorldspaceRegions(ModKey mod, IFormLinkGetter<IWorldspaceGetter> worldspace) {
        return EnumerateWorldspaceRegions(mod)
            .Where(ar => ar.Worldspace.Equals(worldspace));
    }

    public IReadOnlyList<IFormLinkGetter<IRegionGetter>> GetAllowedRegions(ModKey mod, IFormLinkGetter<IWorldspaceGetter> worldspace) {
        return EnumerateWorldspaceRegions(mod, worldspace)
            .SelectMany(f => f.Regions)
            .ToArray();
    }
}
