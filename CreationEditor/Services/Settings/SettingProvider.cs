using Autofac;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Settings;

public sealed class SettingProvider : ISettingProvider, IDisposableDropoff {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();
    public IEnumerable<ISetting> Settings { get; }

    public SettingProvider(
        ILifetimeScope lifetimeScope,
        ILogger logger) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);

        // Get setting classes via reflection
        var settings = typeof(ISetting)
            .GetAllSubClass<ISetting>(newScope.Resolve)
            .ToList();

        // From a static parent by type structure, compile all children for runtime use
        var missingSetting = 0;
        foreach (var setting in settings) {
            if (setting.Parent == null) continue;

            // Find parent setting and yourself as child
            var parent = settings.FirstOrDefault(p => p.GetType() == setting.Parent);
            if (parent != null) {
                parent.Children.Add(setting);
            } else {
                missingSetting++;
                logger.Here().Warning("Setting {Name} couldn't be applied, because it's parent setting {Parent} couldn't be found", setting.Name, setting.Parent);
            }
        }

        logger.Here().Debug("Finished loading {Count} setting(s)", settings.Count - missingSetting);

        // Return root settings
        Settings = settings
            .Where(setting => setting.Parent == null)
            .ToList();
    }

    public void Add(IDisposable disposable) => _disposables.Add(disposable);
    public void Dispose() => _disposables.Dispose();
}
