using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Behavior;

/// <summary>
/// Ambient context for the currently running dock drag operation.
/// Used as fallback to resolve <see cref="DockDragData"/> for cross window drags,
/// where the in-process data transfer may not be marshaled between top level windows.
/// </summary>
public static class DockDragContext {
    private static DockDragData? _current;

    /// <summary>
    /// The drag data of the currently running dock drag, if any.
    /// Drags are modal on the UI thread, so there is at most one at a time.
    /// </summary>
    public static DockDragData? Current => _current;

    public static IDisposable Begin(DockDragData data) {
        _current = data;
        return new DragScope();
    }

    /// <summary>
    /// Resolve the dock drag data from the event, falling back to the ambient context for cross window drags.
    /// </summary>
    public static bool TryGet(DragEventArgs e, [NotNullWhen(true)] out DockDragData? dragData) {
        if (e.DataTransfer.TryGet<DockDragData>(out dragData)) return true;

        dragData = _current;
        return dragData is not null;
    }

    private sealed class DragScope : IDisposable {
        public void Dispose() => _current = null;
    }
}
