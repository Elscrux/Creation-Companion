using CreationEditor.Services.Lifecycle;
namespace CreationEditor.Services.Plugin;

public sealed class PluginServiceStartup : ILifecycleTask {
    private readonly IPluginService _pluginService;
    public PluginServiceStartup(IPluginService pluginService) {
        _pluginService = pluginService;
    }

    public void PreStartup() {}
    public void PostStartupAsync(CancellationToken token) => _pluginService.ReloadPlugins();
    public void OnExit() {}
}
