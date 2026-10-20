using System.Reactive;
using System.Reactive.Disposables;
using CreationEditor.Avalonia.Services.HotKey;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using DynamicData;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Actions.Global;

/// <summary>
/// Central registry of all globally available editor actions.
/// </summary>
public sealed class GlobalActionRegistry : IDisposable {
    private readonly IHotkeyService _hotkeyService;
    private readonly IDockFactory _dockFactory;
    private readonly CompositeDisposable _disposables = [];

    public SourceCache<GlobalAction, string> Actions { get; } = new(x => x.Id);

    public GlobalActionRegistry(
        IEnumerable<IGlobalActionsProvider> providers,
        IHotkeyService hotkeyService,
        IDockFactory dockFactory,
        IPluginService? pluginService) {
        _hotkeyService = hotkeyService;
        _dockFactory = dockFactory;

        Register(providers.SelectMany(x => x.GetActions()));

        if (pluginService is not null) {
            pluginService.PluginsRegistered
                .ObserveOnGui()
                .Subscribe(plugins => {
                    foreach (var plugin in plugins.OfType<IMenuPluginDefinition>()) {
                        Register(CreatePluginAction(plugin));
                    }
                })
                .DisposeWith(_disposables);

            pluginService.PluginsUnregistered
                .ObserveOnGui()
                .Subscribe(plugins => Unregister(plugins.OfType<IMenuPluginDefinition>().Select(x => x.Name)))
                .DisposeWith(_disposables);
        }
    }

    /// <summary>
    /// Register one or more global actions.
    /// </summary>
    /// <param name="actions">Actions to register</param>
    public void Register(params IEnumerable<GlobalAction> actions) {
        var newActions = actions.ToArray();
        Actions.AddOrUpdate(newActions);

        foreach (var newAction in newActions) {
            if (newAction.HotKey is not null) {
                _hotkeyService.Register(newAction.HotKey, () => newAction.Command?.Execute().Subscribe());
            }
        }
    }

    /// <summary>
    /// Unregister global actions by their ids.
    /// </summary>
    /// <param name="ids">Ids of the actions to unregister</param>
    public void Unregister(params IEnumerable<string> ids) {
        var idsToRemove = ids.ToArray();
        foreach (var id in idsToRemove) {
            var action = Actions.Lookup(id);
            if (action.HasValue && action.Value.HotKey is not null) {
                _hotkeyService.Unregister(action.Value.HotKey);
            }
        }

        Actions.RemoveKeys(idsToRemove);
    }

    private GlobalAction CreatePluginAction(IMenuPluginDefinition plugin) => new(
        $"Plugin.{plugin.Name}",
        $"Open {plugin.Name}",
        CreateOpenPluginCommand(plugin),
        "Plugin",
        plugin.Icon,
        50);

    private ReactiveCommand<Unit, Unit> CreateOpenPluginCommand(IMenuPluginDefinition plugin) {
        return ReactiveCommand.Create(() => _dockFactory.OpenFireAndForget(plugin));
    }

    public void Dispose() {
        _disposables.Dispose();
        Actions.Dispose();
    }
}
