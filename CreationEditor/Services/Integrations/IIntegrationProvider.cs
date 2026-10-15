namespace CreationEditor.Services.Integrations;

/// <summary>
/// Discovers all <see cref="IntegrationDescriptor"/>s registered in the DI container
/// (via assembly scanning), grouped by the integration capability they implement.
/// Descriptors are the only way integrations are referenced before they are registered;
/// this provider never instantiates <see cref="IIntegration"/> objects.
/// </summary>
public interface IIntegrationProvider {
    /// <summary>All discovered integration descriptors</summary>
    IReadOnlyList<IntegrationDescriptor> IntegrationDescriptors { get; }

    /// <summary>All discovered integration capability interfaces (subtypes of <see cref="IIntegration"/>)</summary>
    IReadOnlyList<Type> IntegrationTypes { get; }

    /// <summary>Gets the descriptor for a given integration id</summary>
    IntegrationDescriptor GetDescriptor(Guid integrationId);

    /// <summary>Gets all descriptors for a given integration capability type</summary>
    IReadOnlyList<IntegrationDescriptor> GetDescriptors(Type integrationType);
}
