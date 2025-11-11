using Avalonia.Controls;
namespace CreationEditor.Avalonia;

public static class DockExtension {
    extension(Dock dock) {
        public Dock Opposite() {
            return dock switch {
                Dock.Bottom => Dock.Top,
                Dock.Top => Dock.Bottom,
                Dock.Right => Dock.Left,
                Dock.Left => Dock.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(dock), dock, null),
            };
        }
    }
}
