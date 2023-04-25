using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Plugin;

public interface IPluginDefinition {
    public string Name { get; }
    public string Description { get; }
    public Guid Guid { get; }
}

public interface IPlugin<TMod, TModGetter> : IPluginDefinition
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {
    // Lifecycle
    public void OnRegistered(PluginContext<TMod, TModGetter> pluginContext) {}

    public void OnUnregistered() {}
}
