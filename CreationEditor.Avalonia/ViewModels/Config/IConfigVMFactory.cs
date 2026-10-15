using CreationEditor.Services.Config;
namespace CreationEditor.Avalonia.ViewModels.Config;

/// <summary>
/// Creates <see cref="ConfigVM"/> instances for a config definition, optionally seeded with
/// existing config values.
/// </summary>
public interface IConfigVMFactory {
    ConfigVM Create(IConfigDefinition configDefinition, ConfigValues? configValues = null);
}
