using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Services.Settings;
using Newtonsoft.Json;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDocksSetting(IList<StartupDock> docks) : ISettingModel {
    public static readonly StartupDocksSetting Default = new([
        new StartupDock {
            Identifier = DockableTypes.RecordBrowser,
            DockMode = DockMode.Side,
            Dock = Dock.Left,
        },
        new StartupDock {
            Identifier = DockableTypes.CellBrowser,
            DockMode = DockMode.Side,
            Dock = Dock.Right,
        },
        new StartupDock {
            Identifier = DockableTypes.Log,
            DockMode = DockMode.Side,
            Dock = Dock.Bottom,
        },
    ]);

    [JsonProperty] public IList<StartupDock> Docks { get; } = docks;
}
