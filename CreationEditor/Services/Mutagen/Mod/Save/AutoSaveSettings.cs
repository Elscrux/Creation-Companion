using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed partial class AutoSaveSettings : ReactiveObject, ISettingModel {
    [JsonProperty, Reactive] public partial bool OnShutdown { get; set; } = true;
    [JsonProperty, Reactive] public partial bool OnInterval { get; set; } = true;
    [JsonProperty, Reactive] public partial double IntervalInMinutes { get; set; } = 5;
    [JsonProperty, Reactive] public partial int MaxAutSaveCount { get; set; } = 10;
}
