using System.Reactive;
using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Viewport;

public interface IViewportFactory {
    /// <summary>
    /// This viewport factory is capable of creating multiple instances of the same viewport
    /// </summary>
    bool IsMultiInstanceCapable { get; }

    /// <summary>
    /// Create a viewport
    /// </summary>
    /// <returns>Created viewport</returns>
    Control CreateViewport();

    /// <summary>
    /// Emits an event when a viewport is fully initialized
    /// </summary>
    IObservable<Unit> ViewportInitialized { get; }
}
