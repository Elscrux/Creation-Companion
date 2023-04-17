using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Settings.View;
using CreationEditor.Avalonia.Services.Startup;
using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Setting.View;

public sealed record ViewSetting(ViewMode ViewMode);
public sealed class ViewSettingVM : ViewModel, ISetting, ILifecycleTask {
    public static readonly IEnumerable<ViewMode> ViewModes = Enum.GetValues<ViewMode>();

    public string Name => "View";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = new();

    private readonly ResourceDictionary _viewModeResourceDictionary = new();

    private readonly IDictionary<ViewMode, IViewModeTemplate> _viewModeTemplates;

    [JsonProperty]
    [Reactive] public ViewMode ViewMode { get; set; } = ViewMode.Normal;

    public ViewSettingVM(
        ISettingImporter<ViewSetting> settingsImporter) {
        var viewSetting = settingsImporter.Import(this);
        if (viewSetting != null) {
            ViewMode = viewSetting.ViewMode;
        }

        _viewModeTemplates = typeof(IViewModeTemplate)
            .GetAllSubClass<IViewModeTemplate>()
            .ToDictionary(template => template.ViewMode, template => template);
    }

    public void OnStartup() {
        if (Application.Current == null) throw new AppDomainUnloadedException("Application not started successfully");

        Application.Current.Resources.MergedDictionaries.Add(_viewModeResourceDictionary);

        Apply();
    }

    public void OnExit() {}

    public void Apply() {
        if (!_viewModeTemplates.TryGetValue(ViewMode, out var viewModeTemplate)) return;

        _viewModeResourceDictionary.Clear();

        foreach (var (name, value) in viewModeTemplate.Spacings) {
            _viewModeResourceDictionary.AddDeferred(name, x => value);
        }
    }
}
