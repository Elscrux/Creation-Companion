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
}
