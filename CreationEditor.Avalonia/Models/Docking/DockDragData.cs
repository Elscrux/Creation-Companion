using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Models.Docking;

public sealed record DockDragData {
    public required IDockedItem Item { get; set; }
    public Dock? Dock { get; set; }
    public IDockPreview? Preview { get; set; }
}
