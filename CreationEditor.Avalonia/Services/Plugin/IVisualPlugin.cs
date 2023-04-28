using Avalonia.Controls;
using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IVisualPluginDefinition : IPluginDefinition {
    public Control GetControl();
}

public interface IVisualPlugin<TMod, TModGetter> : IVisualPluginDefinition, IPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {}