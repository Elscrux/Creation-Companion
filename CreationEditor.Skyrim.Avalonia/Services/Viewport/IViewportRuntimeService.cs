using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport;

public interface IViewportRuntimeService {
    /// <summary>
    /// Load an interior cell
    /// </summary>
    /// <param name="cell">Cell to load</param>
    void LoadInteriorCell(ICellGetter cell);

    /// <summary>
    /// Load an exterior cell in a worldspace
    /// </summary>
    /// <param name="worldspaceFormKey">Worldspace the cell is in</param>
    /// <param name="cell">Cell to load</param>
    void LoadExteriorCell(FormKey worldspaceFormKey, ICellGetter cell);
}
