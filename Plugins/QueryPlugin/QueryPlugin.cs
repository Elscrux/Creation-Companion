using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using QueryPlugin.ViewModels;
using QueryPlugin.Views;
namespace QueryPlugin;

public sealed class QueryPlugin(
    PluginContext pluginContext,
    Func<QueryPluginVM> queryPluginVMFactory)
    : IMenuPlugin {

    private readonly PluginContext _pluginContext = pluginContext;

    public string Name => "Query";
    public string Description => "Query records.";
    public Guid Guid => new("e07d38ba-10fd-44b9-9a91-f87095ef316b");
    public KeyGesture KeyGesture => new(Key.Q, KeyModifiers.Control);

    public Control GetControl() => new QueryPluginView(queryPluginVMFactory());

    public object GetIcon() => new SymbolIcon { Symbol = Symbol.List };
}
