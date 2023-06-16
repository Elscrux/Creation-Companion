using System.IO.Abstractions;
using System.Reflection;
using Autofac;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Lifecycle;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Plugin;

public sealed record PluginContext<TMod, TModGetter>(
    Version EditorVersion,
    IEditorEnvironment<TMod, TModGetter> EditorEnvironment,
    ILifetimeScope LifetimeScope)
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>;

public interface IPluginService {
    public IReadOnlyList<IPluginDefinition> Plugins { get; }
}

public sealed class PluginService<TMod, TModGetter> : IPluginService, ILifecycleTask
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {
    private const string PluginsFolderName = "Plugins";

    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public IReadOnlyList<IPluginDefinition> Plugins { get; private set; } = null!;

    public PluginContext<TMod, TModGetter> PluginContext { get; }

    public PluginService(
        IFileSystem fileSystem,
        ILogger logger,
        IEditorEnvironment<TMod, TModGetter> editorEnvironment,
        ILifetimeScope lifetimeScope) {
        _fileSystem = fileSystem;
        _logger = logger;

        PluginContext = new PluginContext<TMod, TModGetter>(new Version(1, 0), editorEnvironment, lifetimeScope);
    }

    public void OnStartup() {
        LoadPlugins();
    }

    private void LoadPlugins() {
        // Get application directory
        var applicationDirectory = Path.GetDirectoryName(typeof(PluginService<TMod, TModGetter>).Assembly.Location);
        if (applicationDirectory == null) {
            _logger.Warning("Couldn't load any plugins because the application directory couldn't be found");
            return;
        }

        // Get plugins folder
        var pluginsFolder = Path.Combine(applicationDirectory, PluginsFolderName);
        if (!_fileSystem.Directory.Exists(pluginsFolder)) {
            _logger.Warning("Couldn't load any plugins because the plugins folder {PluginsFolder} doesn't exist", pluginsFolder);
            return;
        }

        // Get plugin paths
        var pluginPaths = _fileSystem.Directory.GetFiles(pluginsFolder, "*.dll");
        if (pluginPaths.Length == 0) {
            _logger.Information("Couldn't load any plugins because there were no plugins in {PluginsFolder}", pluginsFolder);
            return;
        }

        // Collect plugins objects from files
        Plugins = pluginPaths
            .SelectMany(pluginPath => {
                var assembly = Assembly.LoadFrom(pluginPath);
                var plugins = assembly == null ? Array.Empty<IPlugin<TMod, TModGetter>>() : CreatePlugins(assembly).ToArray();

                if (plugins.Length == 0) {
                    _logger.Information("Couldn't load a plugin in file {File} because none of the assembly types implement the {Interface} interface", pluginPath, nameof(IPlugin<TMod, TModGetter>));
                }
                return plugins;
            })
            .NotNull()
            .ToList();

        foreach (var plugin in Plugins.OfType<IPlugin<TMod, TModGetter>>()) {
            plugin.OnRegistered(PluginContext);
        }
    }

    public void OnExit() {}

    private static IEnumerable<IPlugin<TMod, TModGetter>> CreatePlugins(Assembly assembly) {
        foreach (var type in assembly.GetTypes()) {
            if (!typeof(IPlugin<TMod, TModGetter>).IsAssignableFrom(type)) continue;
            if (Activator.CreateInstance(type) is not IPlugin<TMod, TModGetter> result) continue;

            yield return result;
        }
    }
}
