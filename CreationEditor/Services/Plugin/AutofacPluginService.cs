using System.Reactive.Subjects;
using System.Reflection;
using Autofac;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Plugin;

public sealed class AutofacPluginService<TMod, TModGetter>(
    IPluginAssemblyProvider pluginAssemblyProvider,
    ILogger logger,
    ILifetimeScope lifetimeScope)
    : IPluginService, IDisposable
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {

    private readonly List<ILifetimeScope> _pluginScopes = [];
    private readonly PluginContext _pluginContext = new(new Version(1, 0));
    private readonly List<IPlugin> _plugins = [];

    private readonly Subject<IReadOnlyList<IPluginDefinition>> _pluginsLoaded = new();
    public IObservable<IReadOnlyList<IPluginDefinition>> PluginsLoaded => _pluginsLoaded;

    private readonly Subject<IReadOnlyList<IPluginDefinition>> _pluginsUnloaded = new();
    public IObservable<IReadOnlyList<IPluginDefinition>> PluginsUnloaded => _pluginsUnloaded;

    public void ReloadPlugins() {
        UnregisterPlugins();

        var newPlugins = GetPlugins();

        RegisterPlugins(newPlugins);
    }

    private List<IPlugin> GetPlugins() {
        return pluginAssemblyProvider.GetAssemblies()
            .SelectMany(assembly => {
                var plugins = CreatePlugins(assembly).ToList();

                if (plugins.Count == 0) {
                    logger.Here().Warning(
                        "No plugins found in {Assembly} because none of the assembly's types implement {PluginType}",
                        assembly,
                        nameof(IPlugin));
                }
                return plugins;
            })
            .NotNull()
            .ToList();
    }

    private IEnumerable<IPlugin> CreatePlugins(Assembly assembly) {
        var pluginScope = lifetimeScope.BeginLifetimeScope(c => {
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
        _plugins.AddRange(plugins);

        foreach (var plugin in _plugins.OfType<IPlugin>()) {
            plugin.OnRegistered();
        }

        _pluginsLoaded.OnNext(plugins);
    }

    private void UnregisterPlugins() {
        if (_plugins.Count == 0) return;

        foreach (var plugin in _plugins.OfType<IPlugin>()) {
            plugin.OnUnregistered();
        }

        _pluginsUnloaded.OnNext(_plugins);

        _plugins.Clear();
    }

    public void Dispose() {
        UnregisterPlugins();

        lifetimeScope.Dispose();
        foreach (var pluginScope in _pluginScopes) {
            pluginScope.Dispose();
        }
    }
}
