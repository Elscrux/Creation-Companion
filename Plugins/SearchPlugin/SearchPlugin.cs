using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Plugins.Records;
using SearchPlugin.ViewModels;
using SearchPlugin.Views;
namespace SearchPlugin;

public sealed class SearchPlugin<TMod, TModGetter>(
    Func<TextSearchVM<TMod, TModGetter>> textSearchVMFactory,
    PluginContext pluginContext)
    : IMenuPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {

    private readonly PluginContext _pluginContext = pluginContext;

    public string Name => "Search and Replace";
    public string Description => "Search and replace text.";
    public Guid Guid => new("3134c266-5eb1-4671-a42b-9f6b1199b9e5");
    public KeyGesture KeyGesture => new(Key.F, KeyModifiers.Control);
    public DockMode DockMode { get; set; } = DockMode.Side;
    public Dock Dock { get; set; } = Dock.Top;

    public Control GetControl() => new TextSearchView(textSearchVMFactory());

    public object GetIcon() => new FontIcon { Glyph = "🔍" };
}
