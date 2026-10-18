namespace CreationEditor.Avalonia.Models.Docking;

/// <summary>
/// Drop zone of a dock drag target. The <see cref="Center"/> zone docks the dragged
/// items as documents (tabs), the edge zones split the target at that side.
/// </summary>
public enum DockDropZone {
    Left,
    Top,
    Right,
    Bottom,
    Center,
}