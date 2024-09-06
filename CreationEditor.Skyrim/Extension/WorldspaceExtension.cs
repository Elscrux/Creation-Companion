using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class WorldspaceExtension {
    public static IEnumerable<ICellGetter> EnumerateCells(this IWorldspaceGetter worldspace) {
        foreach (var block in worldspace.SubCells) {
            foreach (var subBlock in block.Items) {
                foreach (var cell in subBlock.Items) {
                    yield return cell;
                }
            }
        }
    }

    public static IEnumerable<ICellGetter> EnumerateAllCells(this ILinkCache linkCache, FormKey worldspaceFormKey) {
        return linkCache.ResolveAll<IWorldspaceGetter>(worldspaceFormKey)
            .SelectMany(worldspace => worldspace.EnumerateCells())
            .DistinctBy(x => x.FormKey);
    }

    public static ICellGetter? GetCell(this ILinkCache linkCache, FormKey worldspaceFormKey, P2Int cellCoordinates) {
        foreach (var worldspace in linkCache.ResolveAll<IWorldspaceGetter>(worldspaceFormKey)) {
            foreach (var cell in worldspace.EnumerateCells()) {
                if (cell.Grid is null) continue;

                if (cell.Grid.Point == cellCoordinates) {
                    return cell;
                }
            }
        }

        return null;
    }
}
