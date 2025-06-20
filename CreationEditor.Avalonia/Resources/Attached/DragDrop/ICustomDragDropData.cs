using Avalonia;
namespace CreationEditor.Avalonia.Attached.DragDrop;

public interface ICustomDragDropData<T> {
    static abstract string Data { get; }

    static abstract Func<object?, T>? GetData(AvaloniaObject obj);
    static abstract Action<T>? GetSetData(AvaloniaObject obj);
    static abstract Func<T, bool>? GetCanSetData(AvaloniaObject obj);
}
