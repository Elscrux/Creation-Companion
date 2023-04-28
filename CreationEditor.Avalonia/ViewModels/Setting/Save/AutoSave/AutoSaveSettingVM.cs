using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Settings;
namespace CreationEditor.Avalonia.ViewModels.Setting.Save.AutoSave;

public sealed class AutoSaveSettingVM : ViewModel, ISetting, ILifecycleTask {
    private readonly IAutoSaveService _autoSaveService;

    public string Name => "Auto Save";
    public Type Parent => typeof(SaveSettingVM);
    public List<ISetting> Children { get; } = new();

    public AutoSaveSettings Settings { get; }
    public ISettingModel Model => Settings;

    public AutoSaveSettingVM(
        ISettingImporter<AutoSaveSettings> settingsImporter,
        IAutoSaveService autoSaveService) {
        _autoSaveService = autoSaveService;

        Settings = settingsImporter.Import(this) ?? new AutoSaveSettings();
    }

    public void OnStartup() => _autoSaveService.SetSettings(Settings);

    public void OnExit() {}

    public void Apply() => _autoSaveService.SetSettings(Settings);
}
