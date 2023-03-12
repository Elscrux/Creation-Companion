using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDock {
    [Reactive] public DockElement DockElement { get; set; }
    [Reactive] public DockMode DockMode { get; set; }
    [Reactive] public Dock Dock { get; set; }
}
