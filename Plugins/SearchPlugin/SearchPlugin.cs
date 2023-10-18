using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Plugins.Records;
using SearchPlugin.ViewModels;
using SearchPlugin.Views;
namespace SearchPlugin;

public sealed class SearchPlugin<TMod, TModGetter> : IMenuPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {
    private readonly Func<TextSearchVM<TMod, TModGetter>> _textSearchVMFactory;
    private readonly PluginContext _pluginContext;

    public string Name => "Search and Replace";
    public string Description => "Search and replace text.";
    public Guid Guid => new("3134c266-5eb1-4671-a42b-9f6b1199b9e5");
    public KeyGesture KeyGesture => new(Key.F, KeyModifiers.Control);

    public SearchPlugin(
        Func<TextSearchVM<TMod, TModGetter>> textSearchVMFactory,
        PluginContext pluginContext) {
        _textSearchVMFactory = textSearchVMFactory;
        _pluginContext = pluginContext;
    }

    public Control GetControl() => new TextSearchView(_textSearchVMFactory());

    public object GetIcon() => new SymbolIcon { Symbol = Symbol.Find };
}
