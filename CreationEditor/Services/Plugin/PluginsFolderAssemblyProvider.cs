using System.IO.Abstractions;
using System.Reflection;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Plugin;

public sealed class PluginsFolderAssemblyProvider(
    IFileSystem fileSystem,
    ILogger? logger = null)
    : IPluginAssemblyProvider {

    private const string PluginsFolderName = "Plugins";
    private readonly string _pluginsFolder = Path.Combine(AppContext.BaseDirectory, PluginsFolderName);

    public IEnumerable<Assembly> GetAssemblies() {
        // Check plugins folder
        if (!fileSystem.Directory.Exists(_pluginsFolder)) {
            logger?.Here().Warning("Couldn't load any plugins because the plugins folder {PluginsFolder} doesn't exist", _pluginsFolder);
            return [];
        }

        // Get plugin paths
        var pluginPaths = fileSystem.Directory.GetFiles(_pluginsFolder, "*.dll");
        if (pluginPaths.Length == 0) {
            logger?.Here().Information("Couldn't load any plugins because there were no plugins in {PluginsFolder}", _pluginsFolder);
            return [];
        }

        // Collect assemblies from files
        return pluginPaths
            .Select(Assembly.LoadFrom)
            .NotNull();
    }
}
