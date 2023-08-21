using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IMenuPlugin : IMenuPluginDefinition, IPlugin {}

public interface IMenuPlugin<TMod, TModGetter> : IMenuPlugin, IPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {}
