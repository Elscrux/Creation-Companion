namespace CreationEditor.Services.Plugin;

public interface IPluginService {
    /// <summary>
    /// Emits new plugins that are loaded. This doesn't emit already loaded ones.
    /// </summary>
    IObservable<IReadOnlyList<IPluginDefinition>> PluginsLoaded { get; }

    /// <summary>
    /// Emits plugins that were unloaded.
    /// </summary>
    IObservable<IReadOnlyList<IPluginDefinition>> PluginsUnloaded { get; }

    /// <summary>
    /// Reloads all plugins.
    /// </summary>
    void ReloadPlugins();
}
