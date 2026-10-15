using System.Reflection;
namespace CreationEditor.Services.Config;

public static class ConfigDefinitionExtensions {
    extension(IConfigDefinition configDefinition) {
        public IConfigOption? GetConfigOption(string staticPropertyName) {
            var type = configDefinition.GetType();

            if (type?
                .GetProperty(staticPropertyName, BindingFlags.Static | BindingFlags.Public)?
                .GetValue(null) is IConfigOption configOption) {
                return configOption;
            }

            return null;
        }
    }
}
