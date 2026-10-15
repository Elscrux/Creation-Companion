using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Settings.AssetBrowser;

public sealed partial class AssetBrowserSetting : ReactiveObject, ISettingModel {
    [JsonProperty] [Reactive] public partial Guid? TextureInfoIntegrationId { get; set; } = null;

    [JsonProperty] [Reactive] public partial bool ShowReferencedFiles { get; set; } = true;
    [JsonProperty] [Reactive] public partial bool ShowOrphanedFiles { get; set; } = true;
    [JsonProperty] [Reactive] public partial bool ShowOtherFiles { get; set; } = true;
    [JsonProperty] [Reactive] public partial bool ShowEmptyDirectories { get; set; } = true;
    [JsonProperty] [Reactive] public partial bool ShowIgnoredDirectories { get; set; } = false;
}
