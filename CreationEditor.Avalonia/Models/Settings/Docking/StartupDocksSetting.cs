using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Services.Settings;
using DynamicData.Binding;
using Newtonsoft.Json;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDocksSetting(IList<StartupDock> docks) : ISettingModel {
    public static readonly StartupDocksSetting Default = new(new ObservableCollectionExtended<StartupDock> {
        new() {
            DockElement = DockElement.RecordBrowser,
            DockMode = DockMode.Side,
            Dock = Dock.Left
        },
        new() {
            DockElement = DockElement.CellBrowser,
            DockMode = DockMode.Side,
            Dock = Dock.Right
        },
        new() {
            DockElement = DockElement.Log,
            DockMode = DockMode.Side,
            Dock = Dock.Bottom
        },
    });

    [JsonProperty] public IList<StartupDock> Docks { get; } = docks;
}
