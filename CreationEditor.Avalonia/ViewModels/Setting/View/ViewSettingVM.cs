using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Settings.View;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Settings;
namespace CreationEditor.Avalonia.ViewModels.Setting.View;

public sealed class ViewSettingVM : ViewModel, ISetting, ILifecycleTask {
    public static readonly IEnumerable<ViewMode> ViewModes = Enum.GetValues<ViewMode>();

    public string Name => "View";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = new();

    public ISettingModel Model => Setting;
    public ViewSetting Setting { get; }

    private readonly ResourceDictionary _viewModeResourceDictionary = new();

    private readonly IDictionary<ViewMode, IViewModeTemplate> _viewModeTemplates;

    public ViewSettingVM(
        ISettingImporter<ViewSetting> settingsImporter) {
        Setting = settingsImporter.Import(this) ?? new ViewSetting();

        _viewModeTemplates = typeof(IViewModeTemplate)
            .GetAllSubClasses<IViewModeTemplate>()
            .ToDictionary(template => template.ViewMode, template => template);
    }

    public void OnStartup() {
        if (Application.Current is null) throw new AppDomainUnloadedException("Application not started successfully");

        Application.Current.Resources.MergedDictionaries.Add(_viewModeResourceDictionary);

        Apply();
    }

    public void OnExit() {}

    public void Apply() {
        if (!_viewModeTemplates.TryGetValue(Setting.ViewMode, out var viewModeTemplate)) return;

        _viewModeResourceDictionary.Clear();

        foreach (var (name, value) in viewModeTemplate.Spacings) {
            _viewModeResourceDictionary.AddDeferred(name, _ => value);
        }
    }
}
