using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class CellExtension {
    /// <summary>
    /// Returns all placed objects in a cell, based on the load order.
    /// All references that are overridden by a higher priority mod are excluded.
    /// </summary>
    /// <param name="cell">Cell to get placed from</param>
    /// <param name="linkCache">Link cache to determine the load order</param>
    /// <param name="includeDeleted">Whether to exclude deleted references</param>
    /// <returns>All placed objects in the cell</returns>
    public static IEnumerable<IPlacedGetter> GetAllPlaced(this ICellGetter cell, ILinkCache linkCache, bool includeDeleted = false) {
        var allCells = linkCache.ResolveAll<ICellGetter>(cell.FormKey).ToArray();

        return PlacedObjectsImpl()
            .Where(x => includeDeleted || !x.IsDeleted)
            .DistinctBy(x => x.FormKey);

        IEnumerable<IPlacedGetter> PlacedObjectsImpl() {
            foreach (var cellGetter in allCells) {
                foreach (var placed in cellGetter.Temporary.Concat(cellGetter.Persistent)) {
                    yield return placed;
                }
            }
        }
    }

    /// <summary>
    /// Takes an interior cell and returns all doors that lead to a worldspace.
    /// Doors to worldspaces through connected cells are also included at the end.
    /// </summary>
    /// <param name="cell">Interior cell</param>
    /// <param name="linkCache">Link cache</param>
    /// <returns>Door linking to an interior cell placed in the worldspace</returns>
    public static IEnumerable<IPlacedObjectGetter> GetDoorsToWorldspace(this ICellGetter cell, ILinkCache linkCache) {
        if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) throw new ArgumentException("Cell must be an interior cell", nameof(cell));

        return GetDoorsToWorldspaceImp(cell, linkCache);
    }

    private static IEnumerable<IPlacedObjectGetter> GetDoorsToWorldspaceImp(ICellGetter cell, ILinkCache linkCache) {
        var searchedCells = new HashSet<FormKey> { cell.FormKey };
        var missingCells = new Queue<ICellGetter>([cell]);

        while (missingCells.Count > 0) {
            var currentCell = missingCells.Dequeue();
            searchedCells.Add(currentCell.FormKey);

            foreach (var door in GetDoors(currentCell)) {
                if (door.TeleportDestination is null) continue;
                if (!linkCache.TryResolveSimpleContext<IPlacedObjectGetter>(door.TeleportDestination.Door.FormKey, out var destinationContext)) continue;
                if (destinationContext.Parent?.Record is not ICellGetter destinationCell) continue;

                if ((destinationCell.Flags & Cell.Flag.IsInteriorCell) == 0) {
                    yield return destinationContext.Record;
                } else if (!searchedCells.Contains(destinationCell.FormKey)) {
                    missingCells.Enqueue(destinationCell);
                }
            }
        }
        IEnumerable<IPlacedObjectGetter> GetDoors(ICellGetter c) {
            foreach (var placedObjectGetter in c.Persistent.Concat(c.Temporary).OfType<IPlacedObjectGetter>()) {
                var baseObject = placedObjectGetter.Base.TryResolve(linkCache);
                if (baseObject is not IDoorGetter) continue;

                yield return placedObjectGetter;
            }
        }
    }
}
