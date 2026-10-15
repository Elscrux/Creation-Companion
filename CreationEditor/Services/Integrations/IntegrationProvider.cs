namespace CreationEditor.Services.Integrations;

/// <summary>
/// Discovers all <see cref="IntegrationDescriptor"/>s and integration capability interfaces from
/// the DI container. Descriptors are the only way integrations are referenced before they are
/// registered; this provider never instantiates <see cref="IIntegration"/> objects.
/// </summary>
public sealed class IntegrationProvider : IIntegrationProvider {
    public IReadOnlyList<IntegrationDescriptor> IntegrationDescriptors { get; }
    public IReadOnlyList<Type> IntegrationTypes { get; }

    public IntegrationProvider(IEnumerable<IntegrationDescriptor> descriptors) {
        IntegrationDescriptors = [.. descriptors];

        // Discover all integration capability interfaces (subtypes of IIntegration).
        IntegrationTypes = IntegrationDescriptors
            .Select(descriptor => descriptor.CapabilityType)
            .Distinct()
            .ToArray();
    }

    public IntegrationDescriptor GetDescriptor(Guid integrationId) {
        var descriptor = IntegrationDescriptors.FirstOrDefault(d => d.Id == integrationId);

        if (descriptor is null) {
            throw new ArgumentException($"Unknown integration id '{integrationId}'", nameof(integrationId));
        }

        return descriptor;
    }

    public IReadOnlyList<IntegrationDescriptor> GetDescriptors(Type integrationType)
        => IntegrationDescriptors
            .Where(descriptor => integrationType.IsAssignableFrom(descriptor.CapabilityType))
            .ToArray();
}
