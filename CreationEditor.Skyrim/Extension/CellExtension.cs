using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class CellExtension {
    public static IEnumerable<IPlacedGetter> GetAllPlacedObjects(this ICellGetter cell, ILinkCache linkCache) {
        var allCells = linkCache.ResolveAll<ICellGetter>(cell.FormKey).ToArray();

        return PlacedObjects().DistinctBy(x => x.FormKey);

        IEnumerable<IPlacedGetter> PlacedObjects() {
            foreach (var cellGetter in allCells) {
                foreach (var placed in cellGetter.Temporary.Concat(cellGetter.Persistent)) {
                    yield return placed;
                }
            }
        }
    }
}
