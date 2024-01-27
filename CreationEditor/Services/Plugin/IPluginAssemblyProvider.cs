using System.Reflection;
namespace CreationEditor.Services.Plugin;

public interface IPluginAssemblyProvider {
    IEnumerable<Assembly> GetAssemblies();
}
