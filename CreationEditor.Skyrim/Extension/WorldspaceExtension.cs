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


    /// <summary>
    /// Gets a cell for the specified cell coordinates if it exists in the worldspace.
    /// </summary>
    /// <param name="worldspace">Worldspace to add the cell to</param>
    /// <param name="cellCoordinates">Cell coordinates to get the cell for</param>
    /// <returns>The cell at the specified coordinates or null if it does not exist</returns>
    public static ICellGetter? GetCell(this IWorldspaceGetter worldspace, P2Int cellCoordinates)
    {
        var subBlock = worldspace.GetSubBlock(cellCoordinates);
        return subBlock?.Items.FirstOrDefault(b => b.Grid is not null && b.Grid.Point == cellCoordinates);
    }

    /// <summary>
    /// Gets a block for the specified cell coordinates if it exists.
    /// </summary>
    /// <param name="worldspace">Worldspace to add the block to</param>
    /// <param name="blockCoordinates">Coordinates of the cell to get a block for</param>
    /// <returns>The block for the specified cell coordinates or null if it does not exist</returns>
    public static IWorldspaceBlockGetter? GetBlock(this IWorldspaceGetter worldspace, P2Int blockCoordinates)
    {
        // Formula as it can be seen here https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/CELL
        var blockNumberX = (short)Math.Floor(blockCoordinates.X / 32.0);
        var blockNumberY = (short)Math.Floor(blockCoordinates.Y / 32.0);

        return worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == blockNumberX && b.BlockNumberY == blockNumberY);
    }

    /// <summary>
    /// Gets a subblock for the specified cell coordinates if it exists.
    /// </summary>
    /// <param name="worldspace">Worldspace to get the subblock from</param>
    /// <param name="blockCoordinates">Coordinates of the cell to get a subblock for</param>
    /// <returns>The subblock for the specified cell coordinates or null if it does not exist</returns>
    public static IWorldspaceSubBlockGetter? GetSubBlock(this IWorldspaceGetter worldspace, P2Int blockCoordinates)
    {
        // Formula as it can be seen here https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/CELL
        var subBlockNumberX = (short)Math.Floor(blockCoordinates.X / 8.0);
        var subBlockNumberY = (short)Math.Floor(blockCoordinates.Y / 8.0);

        var block = worldspace.GetBlock(blockCoordinates);
        return block?.Items.FirstOrDefault(b => b.BlockNumberX == subBlockNumberX && b.BlockNumberY == subBlockNumberY);
    }
}
