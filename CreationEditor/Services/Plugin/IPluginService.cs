namespace CreationEditor.Services.Plugin;

public interface IPluginService {
    /// <summary>
    /// Emits new plugins that are registered. This doesn't emit already registered ones.
    /// </summary>
    IObservable<IReadOnlyList<IPluginDefinition>> PluginsRegistered { get; }

    /// <summary>
    /// Emits plugins that were unregistered.
    /// </summary>
    IObservable<IReadOnlyList<IPluginDefinition>> PluginsUnregistered { get; }

    /// <summary>
    /// Loads plugins from the plugin assembly and registers them.
    /// </summary>
    void ReloadPlugins();

    /// <summary>
    /// Updates plugins so only plugins that should be registered in the current context are registered.
    /// </summary>
    void UpdatePluginRegistration();
}
