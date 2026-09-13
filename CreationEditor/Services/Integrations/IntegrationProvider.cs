namespace CreationEditor.Services.Integrations;

/// <summary>
/// Discovers all <see cref="IIntegration"/> types and integration capability interfaces from the
/// DI container.
/// </summary>
public sealed class IntegrationProvider : IIntegrationProvider {
    public IReadOnlyList<IIntegration> Implementations { get; }
    public IReadOnlyList<Type> IntegrationTypes { get; }

    public IntegrationProvider(IEnumerable<IIntegration> implementations) {
        Implementations = implementations.ToList();

        // Discover all integration capability interfaces (subtypes of IIntegration).
        IntegrationTypes = Implementations
            .SelectMany(impl => GetCapabilityInterfaces(impl.GetType()))
            .Distinct()
            .ToList();
    }

    public IIntegration? GetImplementation(Guid id)
        => Implementations.FirstOrDefault(impl => impl.Id == id);

    public IReadOnlyList<IIntegration> GetImplementations(Type integrationType)
        => Implementations.Where(impl => integrationType.IsAssignableFrom(impl.GetType())).ToList();

    private static IEnumerable<Type> GetCapabilityInterfaces(Type type) {
        foreach (var iface in type.GetInterfaces()) {
            if (iface == typeof(IIntegration)) continue;
            if (typeof(IIntegration).IsAssignableFrom(iface)) {
                yield return iface;
            }
        }
    }
}
