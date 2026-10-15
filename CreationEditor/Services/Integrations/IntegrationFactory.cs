using Autofac;
namespace CreationEditor.Services.Integrations;

public sealed class IntegrationFactory(ILifetimeScope lifetimeScope) : IIntegrationFactory {
    public IIntegration CreateIntegration(IntegrationDescriptor descriptor, ConfigValues? configValues = null) {
        IIntegration? integration;
        if (configValues is not null) {
            var parameter = TypedParameter.From(configValues);
            integration = lifetimeScope.ResolveOptionalKeyed<IIntegration>(descriptor.Id, parameter);
        } else {
            integration = lifetimeScope.ResolveOptionalKeyed<IIntegration>(descriptor.Id);
        }

        if (integration is null) {
            throw new InvalidOperationException($"No integration registered for id {descriptor.Id}");
        }

        return integration;
    }
}
