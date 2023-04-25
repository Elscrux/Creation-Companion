using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using SearchPlugin.ViewModels;
namespace SearchPlugin;

public class SearchPlugin : IVisualPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Search and Replace";
    public string Description => "Search and replace text.";
    public Guid Guid => new("3134c266-5eb1-4671-a42b-9f6b1199b9e5");

    private PluginContext<ISkyrimMod, ISkyrimModGetter> _pluginContext = null!;

    public void OnRegistered(PluginContext<ISkyrimMod, ISkyrimModGetter> pluginContext) {
        _pluginContext = pluginContext;
    }

    public Control GetControl() {
        return new Views.TextSearchView(new TextSearchVM(_pluginContext));
    }
    public object GetIcon() {
        return new SymbolIcon { Symbol = Symbol.Find };
    }
}
