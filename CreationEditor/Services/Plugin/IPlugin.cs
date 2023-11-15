using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Plugin;

public interface IPlugin : IPluginDefinition {
    /// <summary>
    /// Called when the plugin is registered. Run startup code here.
    /// </summary>
    public void OnRegistered() {
        // Can be run by plugins
    }

    /// <summary>
    /// Called when the plugin is unregistered. Run cleanup code here.
    /// </summary>
    public void OnUnregistered() {
        // Can be run by plugins
    }
}

public interface IPlugin<TMod, TModGetter> : IPlugin
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>;
