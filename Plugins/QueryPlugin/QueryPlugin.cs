using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using QueryPlugin.ViewModels;
using QueryPlugin.Views;
using Key = Avalonia.Input.Key;
namespace QueryPlugin;

public class QueryPlugin : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Query";
    public string Description => "Query records.";
    public Guid Guid => new("e07d38ba-10fd-44b9-9a91-f87095ef316b");

    public KeyGesture KeyGesture => new(Key.Q, KeyModifiers.Control);

    private PluginContext<ISkyrimMod, ISkyrimModGetter> _pluginContext = null!;

    public void OnRegistered(PluginContext<ISkyrimMod, ISkyrimModGetter> pluginContext) {
        _pluginContext = pluginContext;
    }

    public Control GetControl() {
        return new QueryPluginView(new QueryPluginVM(_pluginContext));
    }

    public object GetIcon() => new SymbolIcon { Symbol = Symbol.List };
}
