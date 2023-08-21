using CreationEditor.Services.Lifecycle;
namespace CreationEditor.Services.Plugin;

public sealed class PluginServiceStartup : ILifecycleTask {
    private readonly IPluginService _pluginService;
    public PluginServiceStartup(IPluginService pluginService) {
        _pluginService = pluginService;
    }

    public void OnStartup() => _pluginService.ReloadPlugins();
    public void OnExit() {}
}
