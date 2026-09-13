using System.Reactive;
using System.Reactive.Subjects;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Settings;
namespace CreationEditor.Avalonia.ViewModels.Integrations;

/// <summary>
/// Settings page + registry for configured integrations. Implements <see cref="ISetting"/> so it
/// appears in the Settings window and persists via the <c>ISetting</c> system, and
/// <see cref="IIntegrationRegistry"/> so the Integrations window and pipeline actions can read
/// and modify the configured integrations.
/// </summary>
public sealed partial class IntegrationsSettingVM : ViewModel, ISetting, IIntegrationRegistry {
    private readonly ISettingExporter _settingExporter;
    private readonly List<RegisteredIntegration> _integrations;

    private readonly Subject<Unit> _changed = new();

    public string Name => "Integrations";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = [];

    public ISettingModel Model => Setting;
    public IntegrationsSetting Setting { get; }

    public IReadOnlyList<RegisteredIntegration> Integrations => _integrations;
    public IObservable<Unit> Changed => _changed;

    public IntegrationsSettingVM(
        ISettingImporter<IntegrationsSetting> settingImporter,
        ISettingExporter settingExporter) {
        _settingExporter = settingExporter;
        Setting = settingImporter.Import(this) ?? new IntegrationsSetting();
        _integrations = Setting.Integrations;
    }

    public void Save(RegisteredIntegration integration) {
        var existing = _integrations.FirstOrDefault(i => i.ImplementationId == integration.ImplementationId);
        if (existing is not null) {
            var index = _integrations.IndexOf(existing);
            _integrations[index] = integration;
        } else {
            _integrations.Add(integration);
        }

        _changed.OnNext(Unit.Default);
    }

    public void Remove(RegisteredIntegration integration) {
        _integrations.Remove(integration);
        _changed.OnNext(Unit.Default);
    }

    public void Persist() {
        Setting.Integrations = _integrations;
        _settingExporter.Export(this);
    }

    public void Apply() { }
}
