using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;

public interface IViewportRuntimeService {
    public void LoadInteriorCell(ICellGetter cell);
    public void LoadExteriorCell(FormKey worldspaceFormKey, ICellGetter cell);
}