using System.Reactive.Subjects;
using System.Reflection;
using Autofac;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Plugin;

public sealed class AutofacPluginService<TMod, TModGetter> : IPluginService, IDisposable
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {

    private readonly IPluginAssemblyProvider _pluginAssemblyProvider;
    private readonly ILogger _logger;
    private readonly ILifetimeScope _lifetimeScope;

    private readonly IDisposableDropoff _disposables = new DisposableBucket();
    private readonly List<ILifetimeScope> _pluginScopes = [];
    private readonly PluginContext _pluginContext = new(new Version(1, 0));
    private readonly List<IPlugin> _registeredPlugins = [];
    private List<IPlugin> _loadedPlugins = [];

    private readonly Subject<IReadOnlyList<IPluginDefinition>> _pluginsRegistered = new();
    public IObservable<IReadOnlyList<IPluginDefinition>> PluginsRegistered => _pluginsRegistered;

    private readonly Subject<IReadOnlyList<IPluginDefinition>> _pluginsUnregistered = new();
    public IObservable<IReadOnlyList<IPluginDefinition>> PluginsUnregistered => _pluginsUnregistered;

    public AutofacPluginService(IPluginAssemblyProvider pluginAssemblyProvider,
        IEditorEnvironment editorEnvironment,
        ILogger logger,
        ILifetimeScope lifetimeScope) {
        _pluginAssemblyProvider = pluginAssemblyProvider;
        _logger = logger;
        _lifetimeScope = lifetimeScope;

        editorEnvironment.LoadOrderChanged
            .Subscribe(_ => UpdatePluginRegistration())
            .DisposeWith(_disposables);
    }

    public void ReloadPlugins() {
        UnregisterAllPlugins();

        _loadedPlugins = GetPlugins();

        RegisterPlugins(_loadedPlugins);
    }

    public void UpdatePluginRegistration() {
        var stalePlugins = _registeredPlugins.Where(p => !p.CanRegister()).ToList();
        UnregisterPlugins(stalePlugins);

        var newPlugins = _loadedPlugins.Except(_registeredPlugins).Where(p => p.CanRegister()).ToList();
        RegisterPlugins(newPlugins);
    }

    private List<IPlugin> GetPlugins() {
        return _pluginAssemblyProvider.GetAssemblies()
            .SelectMany(assembly => {
                var plugins = CreatePlugins(assembly).ToList();

                if (plugins.Count == 0) {
                    _logger.Here().Warning(
                        "No plugins found in {Assembly} because none of the assembly's types implement {PluginType}",
                        assembly,
                        nameof(IPlugin));
                }
                return plugins;
            })
            .WhereNotNull()
            .ToList();
    }

    private IEnumerable<IPlugin> CreatePlugins(Assembly assembly) {
        var pluginScope = _lifetimeScope.BeginLifetimeScope(c => {
            c.RegisterAssemblyModules(assembly);
            c.RegisterInstance(_pluginContext)
                .AsSelf();
        });
        _pluginScopes.Add(pluginScope);

        foreach (var type in assembly.GetTypes()) {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;

            // Use game specific plugin if available
            var pluginType = type.IsAssignableTo(typeof(IPlugin<,>)) || type.IsGenericType
                ? type.MakeGenericType(typeof(TMod), typeof(TModGetter))
                : type;

            if (pluginScope.Resolve(pluginType) is not IPlugin result) continue;

            yield return result;
        }
    }

    private void RegisterPlugins(IReadOnlyList<IPlugin> plugins) {
        var newPlugins = plugins
            .Except(_registeredPlugins)
            .Where(p => p.CanRegister())
            .ToList();

        _registeredPlugins.AddRange(newPlugins);

        foreach (var plugin in newPlugins) {
            plugin.OnRegistered();
        }

        _pluginsRegistered.OnNext(newPlugins);
    }

    private void UnregisterAllPlugins() {
        if (_registeredPlugins.Count == 0) return;

        foreach (var plugin in _registeredPlugins) {
            plugin.OnUnregistered();
        }

        _pluginsUnregistered.OnNext(_registeredPlugins);

        _registeredPlugins.Clear();
    }

    private void UnregisterPlugins(IReadOnlyList<IPlugin> plugins) {
        if (_registeredPlugins.Count == 0) return;

        plugins = plugins.Intersect(_registeredPlugins).ToList();

        foreach (var plugin in plugins) {
            plugin.OnUnregistered();
        }

        _pluginsUnregistered.OnNext(plugins);

        _registeredPlugins.Remove(plugins);
    }

    public void Dispose() {
        UnregisterAllPlugins();

        _lifetimeScope.Dispose();
        _disposables.Dispose();
        foreach (var pluginScope in _pluginScopes) {
            pluginScope.Dispose();
        }
    }
}
