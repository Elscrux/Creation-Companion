using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.Models.Docking;

public sealed record DockDragData {
    public required IDockedItem Item { get; init; }

    /// <summary>
    /// All items participating in this drag.
    /// For single item drags this contains only <see cref="Item"/>,
    /// for floating window drags this contains all items of the floating window.
    /// </summary>
    public IReadOnlyList<IDockedItem> Items { get; init; } = [];

    /// <summary>
    /// The currently previewed drop zone, if any.
    /// </summary>
    public DockDropZone? Zone { get; set; }

    public IDockPreview? Preview { get; set; }
}
