using System.Reactive.Disposables;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using DynamicData.Binding;
using Noggog;
using Serilog;
namespace CreationEditor.Avalonia.Services.Docking;

public sealed class DockableRegistry : IDockableRegistry, IDisposable {
    private readonly ObservableCollectionExtended<IDockableDefinition> _definitions = [];
    private readonly ILogger _logger;
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// All registered dockable definitions. The returned instance is updated in place when
    /// plugins register or unregister, so bindings against it stay live.
    /// </summary>
    public IReadOnlyList<IDockableDefinition> Definitions => _definitions;

    public DockableRegistry(
        IEnumerable<IDockableDefinition> definitions,
        IPluginService? pluginService,
        ILogger logger) {
        _logger = logger;

        foreach (var definition in Validate(definitions, logger)) {
            _definitions.Add(definition);
        }

        if (pluginService is null) return;

        pluginService.PluginsRegistered
            .ObserveOnGui()
            .Subscribe(plugins => Register(plugins.OfType<IMenuPluginDefinition>()))
            .DisposeWith(_disposables);

        pluginService.PluginsUnregistered
            .ObserveOnGui()
            .Subscribe(plugins => Unregister(plugins.OfType<IMenuPluginDefinition>().Select(x => x.Name.Replace(" ", string.Empty))))
            .DisposeWith(_disposables);
    }

    public IDockableDefinition? GetByType(string type) {
        return _definitions.FirstOrDefault(definition => definition.Type == type);
    }

    private void Register(IEnumerable<IDockableDefinition> newDefinitions) {
        foreach (var definition in newDefinitions) {
            if (_definitions.Any(x => x.Type == definition.Type)) {
                _logger.Here().Warning("Trying to register duplicate dockable type {Type} - ignoring {Definition}", definition.Type, definition.GetType().Name);
                continue;
            }

            _definitions.Add(definition);
        }
    }

    private void Unregister(IEnumerable<string> types) {
        foreach (var type in types) {
            var definition = _definitions.FirstOrDefault(x => x.Type == type);
            if (definition is not null) _definitions.Remove(definition);
        }
    }

    private static IEnumerable<IDockableDefinition> Validate(IEnumerable<IDockableDefinition> definitions, ILogger logger) {
        var seen = new HashSet<string>();
        foreach (var definition in definitions) {
            if (!seen.Add(definition.Type)) {
                logger.Here().Warning("Duplicate dockable type {Type} - ignoring {Definition}", definition.Type, definition.GetType().Name);
                continue;
            }

            yield return definition;
        }
    }

    public void Dispose() {
        _disposables.Dispose();
    }
}
