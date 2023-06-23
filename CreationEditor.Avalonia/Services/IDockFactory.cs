using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Services;

public interface IDockFactory {
    /// <summary>
    /// Open a new dock
    /// </summary>
    /// <param name="dockElement">Type of dock element to open</param>
    /// <param name="dockMode">Area to open the dock</param>
    /// <param name="dock">Side to open the dock</param>
    /// <param name="parameter">Optional custom parameter which may be used by individual elements</param>
    void Open(DockElement dockElement, DockMode? dockMode = null, Dock? dock = null, object? parameter = null);
}
