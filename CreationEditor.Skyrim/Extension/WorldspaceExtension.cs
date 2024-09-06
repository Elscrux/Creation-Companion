using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
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
}
