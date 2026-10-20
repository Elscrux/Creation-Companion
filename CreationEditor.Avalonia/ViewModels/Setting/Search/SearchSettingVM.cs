using CreationEditor.Avalonia.Models.Settings.Search;
using CreationEditor.Avalonia.ViewModels.Search;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Search;
using CreationEditor.Services.Settings;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting.Search;

public sealed class SearchSettingVM : ViewModel, ISettingNode, ILifecycleTask {
    public string Name => "Search";
    public Type? Parent => null;
    public List<ISettingNode> Children { get; } = [];

    public ISettingModel Model => Setting;
    public SearchSetting Setting { get; }

    /// <summary>
    /// Scope toggles of the settings page, kept in sync with <see cref="SearchSetting.DefaultScopes"/>.
    /// </summary>
    public IReadOnlyList<SelectableSearchScopeVM> Scopes { get; }

    private readonly SearchVM _searchVM;

    public SearchSettingVM(
        ISettingImporter<SearchSetting> settingsImporter,
        SearchVM searchVM) {
        _searchVM = searchVM;
        Setting = settingsImporter.Import(this) ?? new SearchSetting();

        Scopes = Enum.GetValues<SearchScope>()
            .OrderBy(x => x.DisplayOrder())
            .Select(scope => new SelectableSearchScopeVM(scope, Setting.DefaultScopes.Contains(scope.ToString())))
            .ToArray();

        foreach (var scope in Scopes) {
            scope.WhenAnyValue(x => x.IsSelected)
                .Subscribe(_ => UpdateDefaultScopes())
                .DisposeWith(this);
        }
    }

    private void UpdateDefaultScopes() {
        Setting.DefaultScopes = Scopes
            .Where(x => x.IsSelected)
            .Select(x => x.Scope.ToString())
            .ToList();
    }

    public void PreStartup() {}

    public void PostStartupAsync(CancellationToken token) {
        Apply();
    }

    public void OnExit() {}

    public void Apply() {
        _searchVM.MaxResultsPerCategory = Setting.MaxResultsPerCategory;
        _searchVM.SearchDelayInMilliseconds = Setting.SearchDelayInMilliseconds;

        var defaults = Setting.DefaultScopes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var scope in _searchVM.Scopes) {
            scope.IsSelected = defaults.Contains(scope.Scope.ToString());
        }
    }
}
