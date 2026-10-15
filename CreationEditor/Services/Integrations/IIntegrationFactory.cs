namespace CreationEditor.Services.Integrations;

public interface IIntegrationFactory {
    /// <summary>
    /// Creates an instance of the integration described by the given descriptor, using the provided settings.
    /// </summary>
    /// <param name="descriptor">The descriptor of the integration to create</param>
    /// <param name="configValues">The configuration values to use when creating the integration</param>
    /// <returns>An instance of the integration</returns>
    IIntegration CreateIntegration(IntegrationDescriptor descriptor, ConfigValues? configValues = null);
}
