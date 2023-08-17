using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class AutoSaveSettings : ReactiveObject, ISettingModel {
    [JsonProperty, Reactive] public bool OnShutdown { get; set; } = true;
    [JsonProperty, Reactive] public bool OnInterval { get; set; } = true;
    [JsonProperty, Reactive] public double IntervalInMinutes { get; set; } = 5;
    [JsonProperty, Reactive] public int MaxAutSaveCount { get; set; } = 10;
}
