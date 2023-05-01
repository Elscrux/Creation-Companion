using CreationEditor.Avalonia.ViewModels.Setting.Save;
using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Settings.Save;

public sealed class SaveSettings : ReactiveObject, ISettingModel {
    [JsonProperty] [Reactive] public SaveLocation SaveLocation { get; set; }
    [JsonProperty] [Reactive] public string DataRelativeOrAbsoluteCustomSaveLocation { get; set; } = string.Empty;

    [JsonProperty] [Reactive] public bool RemoveIdenticalToMasterRecords { get; set; } = true;
}
