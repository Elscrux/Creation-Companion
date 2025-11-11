using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class CellExtension {
    /// <param name="cell">Cell to get placed from</param>
    extension(ICellGetter cell) {
        /// <summary>
        /// Returns all placed objects in a cell, based on the load order.
        /// All references that are overridden by a higher priority mod are excluded.
        /// </summary>
        /// <param name="linkCache">Link cache to determine the load order</param>
        /// <param name="includeDeleted">Whether to exclude deleted references</param>
        /// <returns>All placed objects in the cell</returns>
        public IEnumerable<IPlacedGetter> GetAllPlaced(ILinkCache linkCache, bool includeDeleted = false) {
            var allCells = linkCache.ResolveAll<ICellGetter>(cell.FormKey).ToArray();

            return PlacedObjectsImpl()
                .DistinctBy(x => x.FormKey)
                .Where(x => includeDeleted || !x.IsDeleted);

            IEnumerable<IPlacedGetter> PlacedObjectsImpl() {
                foreach (var cellGetter in allCells) {
                    foreach (var placed in cellGetter.Temporary.Concat(cellGetter.Persistent)) {
                        yield return placed;
                    }
                }
            }
        }
        /// <summary>
        /// Returns all doors in a cell, based on the load order.
        /// All references that are overridden by a higher priority mod are excluded.
        /// </summary>
        /// <param name="linkCache">Link cache to determine the load order</param>
        /// <param name="includeDeleted">Whether to exclude deleted doors</param>
        /// <returns>All doors in the cell</returns>
        public IEnumerable<IPlacedObjectGetter> GetDoors(ILinkCache linkCache, bool includeDeleted = false) {
            foreach (var placedObjectGetter in cell.GetAllPlaced(linkCache, includeDeleted).OfType<IPlacedObjectGetter>()) {
                var baseObject = placedObjectGetter.Base.TryResolve(linkCache);
                if (baseObject is not IDoorGetter) continue;

                yield return placedObjectGetter;
            }
        }
        /// <summary>
        /// Takes an interior cell and returns all doors that lead to a worldspace.
        /// Doors to worldspaces through connected cells are also included at the end.
        /// </summary>
        /// <param name="linkCache">Link cache</param>
        /// <returns>Door linking to an interior cell placed in the worldspace</returns>
        public IEnumerable<IPlacedObjectGetter> GetDoorsToWorldspace(ILinkCache linkCache) {
            if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) throw new ArgumentException("Cell must be an interior cell", nameof(cell));

            return GetDoorsToWorldspaceImp(cell, linkCache);
        }
    }

    private static IEnumerable<IPlacedObjectGetter> GetDoorsToWorldspaceImp(ICellGetter cell, ILinkCache linkCache) {
        var searchedCells = new HashSet<FormKey> { cell.FormKey };
        var missingCells = new Queue<ICellGetter>([cell]);

        while (missingCells.Count > 0) {
            var currentCell = missingCells.Dequeue();
            searchedCells.Add(currentCell.FormKey);

            foreach (var door in GetDoors(currentCell, linkCache)) {
                if (door.TeleportDestination is null
                 || !linkCache.TryResolveSimpleContext<IPlacedObjectGetter>(door.TeleportDestination.Door.FormKey, out var destinationContext)
                 || destinationContext.Parent?.Record is not ICellGetter destinationCell) continue;

                if ((destinationCell.Flags & Cell.Flag.IsInteriorCell) == 0) {
                    yield return destinationContext.Record;
                } else if (!searchedCells.Contains(destinationCell.FormKey)) {
                    missingCells.Enqueue(destinationCell);
                }
            }
        }
    }

    /// <param name="cell">Interior cell to start from</param>
    extension(ICellGetter cell) {
        public IWorldspaceGetter? GetWorldspace(ILinkCache linkCache) {
            if (cell.Flags.HasFlag(Cell.Flag.IsInteriorCell)) return null;
            if (!linkCache.TryResolveSimpleContext(cell, out var cellContext)) return null;

            if (cellContext.TryGetParent<IWorldspaceGetter>(out var worldspace)) {
                return worldspace;
            }

            return null;
        }
        public bool IsInteriorCell()
        {
            return (cell.Flags & Cell.Flag.IsInteriorCell) != 0;
        }
        public bool IsExteriorCell()
        {
            return (cell.Flags & Cell.Flag.IsInteriorCell) == 0;
        }
        public bool IsPublic()
        {
            return (cell.Flags & Cell.Flag.PublicArea) != 0;
        }
        /// <summary>
        /// Finds all doors from a given interior cell to the next exterior cell.
        /// Linked interior cells are traversed recursively until an exterior cell is found.
        /// </summary>
        /// <param name="linkCache">Link cache to resolve cell links</param>
        /// <returns>All doors leading to an exterior cell</returns>
        public IEnumerable<IModContext<IPlacedObjectGetter>> GetExteriorDoorsGoingIntoInteriorRecursively(ILinkCache linkCache)
        {
            HashSet<FormKey> visitedCells = [cell.FormKey];
            var queue = new Queue<ICellGetter>();
            queue.Enqueue(cell);

            while (queue.Count > 0)
            {
                var currentCell = queue.Dequeue();

                foreach (var placedObject in currentCell.GetAllPlaced(linkCache).OfType<IPlacedObjectGetter>())
                {
                    // Has a teleport destination
                    if (placedObject.TeleportDestination is null || placedObject.TeleportDestination.Door.IsNull) continue;

                    // Teleport destination is a door
                    if (!linkCache.TryResolve<IDoorGetter>(placedObject.Base.FormKey, out _)) continue;

                    if (placedObject.TeleportDestination.Door.TryResolveSimpleContext(linkCache, out var destinationDoor)
                     && destinationDoor.Parent?.Record is ICellGetter destinationCell)
                    {
                        if (destinationCell.IsInteriorCell())
                        {
                            if (visitedCells.Add(destinationCell.FormKey))
                            {
                                queue.Enqueue(destinationCell);
                            }
                        }
                        else
                        {
                            yield return destinationDoor;
                        }
                    }
                }
            }
        }
    }
}
