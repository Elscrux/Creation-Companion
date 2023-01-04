using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Behavior;

public sealed record DockDragData {
    public required IDockedItem Item { get; set; }
    public Dock? Dock { get; set; }
    public IDockPreview? Preview { get; set; }
}
