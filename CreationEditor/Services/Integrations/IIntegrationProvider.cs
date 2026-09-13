namespace CreationEditor.Services.Integrations;

/// <summary>
/// Discovers all <see cref="IIntegration"/> types registered in the DI container
/// (via assembly scanning), grouped by the integration capability they implement.
/// </summary>
public interface IIntegrationProvider {
    /// <summary>All discovered integration implementations.</summary>
    IReadOnlyList<IIntegration> Implementations { get; }

    /// <summary>All discovered integration capability interfaces (subtypes of <see cref="IIntegration"/>).</summary>
    IReadOnlyList<Type> IntegrationTypes { get; }

    /// <summary>Gets an implementation by its stable id.</summary>
    IIntegration? GetImplementation(Guid id);

    /// <summary>Gets all implementations for a given integration capability type.</summary>
    IReadOnlyList<IIntegration> GetImplementations(Type integrationType);
}
