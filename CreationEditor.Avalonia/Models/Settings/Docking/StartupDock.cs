using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDock {
    public DockElement DockElement { get; set; }
    public DockMode DockMode { get; set; }
    public Dock Dock { get; set; }
}
