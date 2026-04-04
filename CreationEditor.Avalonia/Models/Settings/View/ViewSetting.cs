using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Settings.View;

public sealed partial class ViewSetting : ReactiveObject, ISettingModel {
    [JsonProperty, Reactive] public partial ViewMode ViewMode { get; set; }
}
