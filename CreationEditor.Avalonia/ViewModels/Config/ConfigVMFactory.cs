using CreationEditor.Services.Config;
namespace CreationEditor.Avalonia.ViewModels.Config;

/// <summary>
/// Wraps the DI-resolved <see cref="ConfigVM"/> factory func so consumers don't depend on the
/// raw <c>Func&lt;IConfigDefinition, ConfigValues, ConfigVM&gt;</c> signature.
/// </summary>
public sealed class ConfigVMFactory(Func<IConfigDefinition, ConfigValues, ConfigVM> factory) : IConfigVMFactory {
    public ConfigVM Create(IConfigDefinition configDefinition, ConfigValues? configValues = null) =>
        factory(configDefinition, configValues ?? new ConfigValues());
}
