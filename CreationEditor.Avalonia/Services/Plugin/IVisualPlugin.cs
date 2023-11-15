using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IVisualPlugin<TMod, TModGetter> : IVisualPluginDefinition, IPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>;
