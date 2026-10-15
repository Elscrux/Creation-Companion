using System.Reactive;
namespace CreationEditor.Services.Integrations;

/// <summary>
/// Holds the user's registered integrations (added tools with install path + config) and
/// persists them via the <c>ISetting</c> system. Emits <see cref="IntegrationsChanged"/> whenever the set of
/// registered integrations changes.
/// </summary>
public interface IIntegrationRegistry {
    /// <summary>All registered integrations</summary>
    IReadOnlyList<IntegrationRegistration> IntegrationRegistrations { get; }

    /// <summary>All registered integrations that are currently registered</summary>
    IReadOnlyList<IIntegration> Integrations { get; }

    /// <summary>Emits whenever the registered integrations change</summary>
    IObservable<Unit> IntegrationsChanged { get; }

    /// <summary>Adds or updates a registered integration</summary>
    void Register(IntegrationRegistration integrationRegistration);

    /// <summary>Removes a registered integration</summary>
    void Unregister(Guid integrationId);

    /// <summary>
    /// Gets the registered integration for a given integration id, or null if not found.
    /// </summary>
    /// <param name="integrationId">Id of the registered integration to retrieve</param>
    /// <returns>The registered integration, or null if not found</returns>
    IIntegration? TryGetIntegration(Guid integrationId);

    /// <summary>
    /// Gets the registered integration for a given integration id, cast to the specified capability type.
    /// </summary>
    /// <param name="integrationId">Id of the registered integration to retrieve</param>
    /// <typeparam name="T">The capability interface type to cast the integration to (e.g. <c>IBSAArchiveCreator</c>)</typeparam>
    /// <returns>The registered integration cast to the specified capability type, or null if not found or not of that type</returns>
    T? TryGetIntegration<T>(Guid integrationId) where T : IIntegration;

    /// <summary>
    /// Gets all registered integrations that implement the specified capability type.
    /// </summary>
    /// <param name="capabilityType">The capability interface type to filter integrations by</param>
    /// <returns>A list of registered integrations that implement the specified capability type</returns>
    IReadOnlyList<IIntegration> GetIntegrationsForCapability(Type capabilityType);
}
