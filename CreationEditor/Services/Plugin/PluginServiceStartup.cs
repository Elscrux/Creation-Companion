using CreationEditor.Services.Lifecycle;
namespace CreationEditor.Services.Plugin;

public sealed class PluginServiceStartup(IPluginService pluginService) : ILifecycleTask {
    public void PreStartup() {}
    public void PostStartupAsync(CancellationToken token) => pluginService.ReloadPlugins();
    public void OnExit() {}
}
