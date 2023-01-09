using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Models.Docking;

public sealed record DockDragData {
    public required IDockedItem Item { get; init; }
    public Dock? Dock { get; set; }
    public IDockPreview? Preview { get; set; }
}
