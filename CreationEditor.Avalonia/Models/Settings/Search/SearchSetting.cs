using CreationEditor.Services.Search;
using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Settings.Search;

public sealed partial class SearchSetting : ReactiveObject, ISettingModel {
    [JsonProperty, Reactive] public partial int MaxResultsPerCategory { get; set; } = 8;
    [JsonProperty, Reactive] public partial int SearchDelayInMilliseconds { get; set; } = 150;

    // ObjectCreationHandling.Replace: Newtonsoft would otherwise reuse the default-filled list
    // and append the JSON entries, instead of only keeping the scopes saved in the file
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DefaultScopes { get; set; } = [.. Enum.GetNames<SearchScope>()];
}
