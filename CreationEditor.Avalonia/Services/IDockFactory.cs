using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
namespace CreationEditor.Avalonia.Services;

public interface IDockFactory {
    /// <summary>
    /// Open a new dock
    /// </summary>
    /// <param name="dockable">Dockable to open</param>
    /// <param name="dockMode">Area to open the dock</param>
    /// <param name="dock">Side to open the dock</param>
    /// <param name="parameter">Optional custom parameter which may be used by individual elements</param>
    /// <returns>The task that opens the dock</returns>
    Task Open(IDockableDefinition dockable, DockMode? dockMode = null, Dock? dock = null, object? parameter = null);

    /// <summary>
    /// Open a new dock and ignore the result
    /// </summary>
    /// <param name="dockable">Dockable to open</param>
    /// <param name="dockMode">Area to open the dock</param>
    /// <param name="dock">Side to open the dock</param>
    /// <param name="parameter">Optional custom parameter which may be used by individual elements</param>
    void OpenFireAndForget(IDockableDefinition dockable, DockMode? dockMode = null, Dock? dock = null, object? parameter = null);

    /// <summary>
    /// Retrieve a dock of that kind if one is already open
    /// </summary>
    /// <param name="dockable">Dockable to look for</param>
    /// <returns>The dock</returns>
    Control? TryGetDock(IDockableDefinition dockable);

    /// <summary>
    /// Retrieve a dock of that kind if one is already open, or open a new one
    /// </summary>
    /// <param name="dockable">Dockable to open</param>
    /// <param name="dockMode">Area to open the dock</param>
    /// <param name="dock">Side to open the dock</param>
    /// <param name="parameter">Optional custom parameter which may be used by individual elements</param>
    /// <returns>The dock</returns>
    Task<Control> GetOrOpenDock(IDockableDefinition dockable, DockMode? dockMode = null, Dock? dock = null, object? parameter = null);
}
