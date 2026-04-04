using CreationEditor.Avalonia.ViewModels.Setting.Save;
using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Settings.Save;

public sealed partial class SaveSettings : ReactiveObject, ISettingModel {
    [JsonProperty, Reactive] public partial SaveLocation SaveLocation { get; set; }
    [JsonProperty, Reactive] public partial string DataRelativeOrFullCustomSaveLocation { get; set; } = string.Empty;

    [JsonProperty, Reactive] public partial bool RemoveIdenticalToMasterRecords { get; set; } = true;
}
