namespace CreationEditor.Avalonia.Models.Docking;

public sealed class DockException : Exception {
    public DockException() {}
    public DockException(string message) : base(message) {}
    public DockException(string message, Exception inner) : base(message, inner) {}
}
