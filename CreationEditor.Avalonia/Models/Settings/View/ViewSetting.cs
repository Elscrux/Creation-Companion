using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Settings.View;

public sealed class ViewSetting : ReactiveObject, ISettingModel     {
    [JsonProperty] [Reactive] public ViewMode ViewMode { get; set; }
}
