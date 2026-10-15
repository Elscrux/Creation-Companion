using CreationEditor.Services.Config;
namespace CreationEditor.Services.Integrations;

public static class IntegrationExtensions {
    extension(IIntegration integration) {
        public IConfigOption? GetRegistrationConfigOption(string staticPropertyName) =>
            integration.Descriptor.RegistrationConfigDefinition?.GetConfigOption(staticPropertyName);

        public IConfigOption? GetRuntimeConfigOption(string staticPropertyName) =>
            integration.Descriptor.RuntimeConfigDefinition?.GetConfigOption(staticPropertyName);
    }
}
