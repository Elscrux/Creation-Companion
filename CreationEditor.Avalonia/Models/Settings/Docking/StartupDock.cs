using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDock {
    public string Identifier { get; set; } = string.Empty;
    public DockMode DockMode { get; set; }
    public Dock Dock { get; set; }
}
