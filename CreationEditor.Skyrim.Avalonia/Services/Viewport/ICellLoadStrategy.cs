using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport;

public interface ICellLoadStrategy {
    void LoadCell(ICellGetter cell);
}
