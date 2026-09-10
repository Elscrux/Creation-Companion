using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Settings.View;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Settings;
using FluentAvalonia.Styling;
namespace CreationEditor.Avalonia.ViewModels.Setting.View;

public sealed class ViewSettingVM : ViewModel, ISetting, ILifecycleTask {
    public static readonly IEnumerable<ViewMode> ViewModes = Enum.GetValues<ViewMode>();
    public static readonly IEnumerable<PlatformThemeVariant> Themes = Enum.GetValues<PlatformThemeVariant>();

    public string Name => "View";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = [];

    public ISettingModel Model => Setting;
    public ViewSetting Setting { get; }

    private readonly ResourceDictionary _viewModeResourceDictionary = new();
    private readonly Dictionary<ViewMode, IViewModeTemplate> _viewModeTemplates;
    private readonly Dictionary<PlatformThemeVariant, ThemeVariant> _themeVariantMapping = new() {
        { PlatformThemeVariant.Light, ThemeVariant.Light },
        { PlatformThemeVariant.Dark, ThemeVariant.Dark }
    };

    public ViewSettingVM(
        ISettingImporter<ViewSetting> settingsImporter) {
        Setting = settingsImporter.Import(this) ?? new ViewSetting();

        _viewModeTemplates = typeof(IViewModeTemplate)
            .GetAllSubClasses<IViewModeTemplate>()
            .ToDictionary(template => template.ViewMode, template => template);
    }

    public void PreStartup() {}

    public void PostStartupAsync(CancellationToken token) {
        Apply();
    }

    public void OnExit() {}

    public void Apply() {
        if (Application.Current is null) throw new AppDomainUnloadedException("Application not started successfully");

        ApplyViewMode(Application.Current);
        ApplyTheme(Application.Current);
    }

    public void ApplyViewMode(Application app) {
        if (!_viewModeTemplates.TryGetValue(Setting.ViewMode, out var viewModeTemplate)) return;

        app.Resources.MergedDictionaries.Remove(_viewModeResourceDictionary);

        _viewModeResourceDictionary.Clear();

        foreach (var (name, value) in viewModeTemplate.All) {
            _viewModeResourceDictionary.Add(name, value);
        }

        Dispatcher.UIThread.Post(() => app.Resources.MergedDictionaries.Add(_viewModeResourceDictionary));
    }

    private void ApplyTheme(Application app) {
        var style = app.Styles.OfType<FluentAvaloniaTheme>().FirstOrDefault();
        if (style is null) return;

        Dispatcher.UIThread.Post(() => {
            style.PreferSystemTheme = Setting.UseSystemTheme;
            if (!Setting.UseSystemTheme) {
                if (!_themeVariantMapping.TryGetValue(Setting.Theme, out var themeVariant)) return;

                if (app.RequestedThemeVariant != themeVariant) {
                    app.RequestedThemeVariant = themeVariant;
                }
            }

            style.CustomAccentColor = Setting.UseCustomAccessColor
                ? Setting.AccentColor
                : null;
        });
    }
}
