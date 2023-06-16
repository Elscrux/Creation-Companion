using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
namespace DLCMapper; 

public class VanillaDuplicateCleanerPlugin : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Vanilla Duplicate Cleaner";
    public string Description => "Cleans mods from vanilla duplicates.";
    public Guid Guid => new("2179f861-1934-41e7-b612-b80484542c2c");

    private PluginContext<ISkyrimMod, ISkyrimModGetter> _pluginContext = null!;

    public void OnRegistered(PluginContext<ISkyrimMod, ISkyrimModGetter> pluginContext) {
        _pluginContext = pluginContext;
    }

    public Control GetControl() {
        var vanillaDuplicateCleanerVM = new VanillaDuplicateCleanerVM(_pluginContext);

        return new VanillaDuplicateCleanerView(vanillaDuplicateCleanerVM);
    }

    public KeyGesture? KeyGesture => null;

    public object GetIcon() {
        return new SymbolIcon { Symbol = Symbol.Clear };
    }
}