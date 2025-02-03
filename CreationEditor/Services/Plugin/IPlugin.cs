using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Plugin;

public interface IPlugin : IPluginDefinition {
    /// <summary>
    /// Called to determine if the plugin can be registered.
    /// </summary>
    bool CanRegister() {
        // Can be overridden by plugins
        return true;
    }

    /// <summary>
    /// Called when the plugin is registered. Run startup code here.
    /// </summary>
    void OnRegistered() {
        // Can be run by plugins
    }

    /// <summary>
    /// Called when the plugin is unregistered. Run cleanup code here.
    /// </summary>
    void OnUnregistered() {
        // Can be run by plugins
    }
}

public interface IPlugin<TMod, TModGetter> : IPlugin
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>;
