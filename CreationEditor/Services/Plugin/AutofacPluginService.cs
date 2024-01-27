using System.IO.Abstractions;
using System.Reactive.Subjects;
using System.Reflection;
using Autofac;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Plugin;

public sealed class AutofacPluginService<TMod, TModGetter>(
    IFileSystem fileSystem,
    ILogger logger,
    ILifetimeScope lifetimeScope)
    : IPluginService, IDisposable
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {
    private const string PluginsFolderName = "Plugins";

    private readonly List<ILifetimeScope> _pluginScopes = [];
    private readonly PluginContext _pluginContext = new(new Version(1, 0));
    private readonly List<IPlugin> _plugins = [];

    private readonly Subject<IReadOnlyList<IPluginDefinition>> _pluginsLoaded = new();
    public IObservable<IReadOnlyList<IPluginDefinition>> PluginsLoaded => _pluginsLoaded;

    private readonly Subject<IReadOnlyList<IPluginDefinition>> _pluginsUnloaded = new();
    public IObservable<IReadOnlyList<IPluginDefinition>> PluginsUnloaded => _pluginsUnloaded;

    public void ReloadPlugins() {
        UnregisterPlugins();

        var newPlugins = CollectPlugins(Path.Combine(AppContext.BaseDirectory, PluginsFolderName));

        RegisterPlugins(newPlugins);
    }

    private IPlugin[] CollectPlugins(string pluginsFolder) {
        // Check plugins folder
        if (!fileSystem.Directory.Exists(pluginsFolder)) {
            logger.Here().Warning("Couldn't load any plugins because the plugins folder {PluginsFolder} doesn't exist", pluginsFolder);
            return [];
        }

        // Get plugin paths
        var pluginPaths = fileSystem.Directory.GetFiles(pluginsFolder, "*.dll");
        if (pluginPaths.Length == 0) {
            logger.Here().Information("Couldn't load any plugins because there were no plugins in {PluginsFolder}", pluginsFolder);
            return [];
        }

        // Collect plugins objects from files
        return pluginPaths
            .SelectMany(pluginPath => {
                var assembly = Assembly.LoadFrom(pluginPath);
                var plugins = assembly is null ? Array.Empty<IPlugin>() : CreatePlugins(assembly).ToArray();

                if (plugins.Length == 0) {
                    logger.Here().Information("Couldn't load a plugin in file {File} because none of the assembly types implement the {Interface} interface", pluginPath, nameof(IPlugin));
                }
                return plugins;
            })
            .NotNull()
            .ToArray();
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
