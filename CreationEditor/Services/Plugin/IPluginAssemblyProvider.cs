using System.Reflection;
namespace CreationEditor.Services.Plugin;

public interface IPluginAssemblyProvider {
    /// <summary>
    /// Get all assemblies that contain plugins
    /// </summary>
    /// <returns>Plugin assemblies</returns>
    IEnumerable<Assembly> GetAssemblies();
}
