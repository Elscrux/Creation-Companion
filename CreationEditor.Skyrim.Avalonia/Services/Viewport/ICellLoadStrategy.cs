using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport;

public interface ICellLoadStrategy {
    /// <summary>
    /// Load the given cell into the viewport.
    /// </summary>
    /// <param name="cell">Cell to load</param>
    void LoadCell(ICellGetter cell);
}
