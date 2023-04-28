using CreationEditor.Services.Settings;
using Newtonsoft.Json;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDocksSetting : ISettingModel {
    [JsonProperty] public IList<StartupDock> Docks { get; }

    public StartupDocksSetting(IList<StartupDock> docks) {
        Docks = docks;
    }
}
