using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Settings.View;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Settings;
namespace CreationEditor.Avalonia.ViewModels.Setting.View;

public sealed class ViewSettingVM : ViewModel, ISetting, ILifecycleTask {
    public static readonly IEnumerable<ViewMode> ViewModes = Enum.GetValues<ViewMode>();

    public string Name => "View";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = [];

    public ISettingModel Model => Setting;
    public ViewSetting Setting { get; }

    private readonly ResourceDictionary _viewModeResourceDictionary = new();
    private readonly Dictionary<ViewMode, IViewModeTemplate> _viewModeTemplates;

    public ViewSettingVM(
        ISettingImporter<ViewSetting> settingsImporter) {
        Setting = settingsImporter.Import(this) ?? new ViewSetting();

        _viewModeTemplates = typeof(IViewModeTemplate)
            .GetAllSubClasses<IViewModeTemplate>()
            .ToDictionary(template => template.ViewMode, template => template);
    }

    public void PreStartup() {}

    public void PostStartupAsync(CancellationToken token) {
        if (Application.Current is null) throw new AppDomainUnloadedException("Application not started successfully");

        Apply();
    }

    public void OnExit() {}

    public void Apply() {
        if (Application.Current is null) throw new AppDomainUnloadedException("Application not started successfully");
        if (!_viewModeTemplates.TryGetValue(Setting.ViewMode, out var viewModeTemplate)) return;

        Application.Current.Resources.MergedDictionaries.Remove(_viewModeResourceDictionary);

        _viewModeResourceDictionary.Clear();

        foreach (var (name, value) in viewModeTemplate.All) {
            _viewModeResourceDictionary.Add(name, value);
        }

        Dispatcher.UIThread.Post(() => Application.Current.Resources.MergedDictionaries.Add(_viewModeResourceDictionary));
    }
}
