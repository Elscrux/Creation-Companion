using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Extension;

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
        var visitedCells = new HashSet<FormKey>();

        foreach (var worldspace in linkCache.ResolveAll<IWorldspaceGetter>(worldspaceFormKey)) {
            foreach (var cell in worldspace.EnumerateCells()) {
                var cellFormKey = cell.FormKey;
                if (visitedCells.Contains(cellFormKey)) continue;

                visitedCells.Add(cellFormKey);
                yield return cell;
            }
        }
    }
}
