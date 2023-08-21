using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Plugin;

public interface IPlugin : IPluginDefinition {
    // Lifecycle
    public void OnRegistered() {}

    public void OnUnregistered() {}
}

public interface IPlugin<TMod, TModGetter> : IPlugin
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {}
