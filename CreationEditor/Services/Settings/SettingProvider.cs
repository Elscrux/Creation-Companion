using Noggog;
using Serilog;
namespace CreationEditor.Services.Settings;

public sealed class SettingProvider : ISettingProvider, IDisposableDropoff {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();
    public IEnumerable<ISetting> Settings { get; }

    public SettingProvider(
        IEnumerable<ISetting> settings,
        ILogger logger) {
        // Get setting classes via reflection
        var settingsList = settings.ToList();

        // From a static parent by type structure, compile all children for runtime use
        var missingSetting = 0;
        foreach (var setting in settingsList) {
            if (setting.Parent is null) continue;

            // Find parent setting and yourself as child
            var parent = settingsList.Find(p => p.GetType() == setting.Parent);
            if (parent is not null) {
                parent.Children.Add(setting);
            } else {
                missingSetting++;
                logger.Here().Warning("Setting {Name} couldn't be applied, because it's parent setting {Parent} couldn't be found", setting.Name, setting.Parent);
            }
        }

        logger.Here().Debug("Finished loading {Count} setting(s)", settingsList.Count - missingSetting);

        // Return root settings
        Settings = settingsList
            .Where(setting => setting.Parent is null)
            .ToList();
    }

    public void Add(IDisposable disposable) => _disposables.Add(disposable);
    public void Dispose() => _disposables.Dispose();
}
