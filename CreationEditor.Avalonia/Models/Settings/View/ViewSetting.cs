using Avalonia.Media;
using Avalonia.Platform;
using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Settings.View;

public sealed partial class ViewSetting : ReactiveObject, ISettingModel {
    [JsonProperty, Reactive] public partial bool UseCustomAccessColor { get; set; } = false;
    [JsonProperty, Reactive] public partial Color? AccentColor { get; set; } = null;
    [JsonProperty, Reactive] public partial bool UseSystemTheme { get; set; } = true;
    [JsonProperty, Reactive] public partial PlatformThemeVariant Theme { get; set; }
    [JsonProperty, Reactive] public partial ViewMode ViewMode { get; set; }
}
