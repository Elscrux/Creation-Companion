using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Services.Settings;
using DynamicData.Binding;
using Newtonsoft.Json;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDocksSetting(IList<StartupDock> docks) : ISettingModel {
    public static readonly StartupDocksSetting Default = new([
        new StartupDock {
            DockElement = DockElement.RecordBrowser,
            DockMode = DockMode.Side,
            Dock = Dock.Left,
        },
        new StartupDock {
            DockElement = DockElement.CellBrowser,
            DockMode = DockMode.Side,
            Dock = Dock.Right,
        },
        new StartupDock {
            DockElement = DockElement.Log,
            DockMode = DockMode.Side,
            Dock = Dock.Bottom,
        },
    ]);

    [JsonProperty] public IList<StartupDock> Docks { get; } = docks;
}
