using Avalonia.Controls;
namespace CreationEditor.Avalonia;

public static class DockExtension {
    public static Dock Opposite(this Dock dock) {
        return dock switch {
            Dock.Bottom => Dock.Top,
            Dock.Top => Dock.Bottom,
            Dock.Right => Dock.Left,
            Dock.Left => Dock.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(dock), dock, null)
        };
    }
}
