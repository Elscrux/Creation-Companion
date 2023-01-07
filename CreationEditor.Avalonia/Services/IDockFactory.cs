using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Models.Settings;
namespace CreationEditor.Avalonia.Services;

public interface IDockFactory {
    public void Open(DockElement dockElement, DockMode? dockMode = null, Dock? dock = null);
}
