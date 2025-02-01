using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Settings.View;

public sealed class ViewSetting : ReactiveObject, ISettingModel {
    [JsonProperty, Reactive] public ViewMode ViewMode { get; set; }
}
