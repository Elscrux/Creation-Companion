using CreationEditor.Services.Settings;
using Newtonsoft.Json;
namespace CreationEditor.Avalonia.Models.Settings.Docking;

public sealed class StartupDocksSetting(IList<StartupDock> docks) : ISettingModel {
    [JsonProperty] public IList<StartupDock> Docks { get; } = docks;
}
