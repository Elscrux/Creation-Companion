using CreationEditor.Services.Config;
using CreationEditor.Services.Integrations.Archive;
using CreationEditor.Services.Integrations.Lip;
using CreationEditor.Services.Integrations.Sources;
namespace CreationEditor.Services.Integrations;

/// <summary>
/// A descriptor for an integration, containing its basic information and the metadata needed to
/// register and run it. Descriptors are the only way integrations are referenced before they are
/// registered; the concrete <see cref="IIntegration"/> object is only created after registration.
/// </summary>
/// <param name="Id">Stable identifier used to persist and rehydrate the integration</param>
/// <param name="DisplayName">Display name shown in the UI</param>
/// <param name="Description">Short description shown in the UI</param>
/// <param name="CategoryName">Human-readable category name</param>
/// <param name="CapabilityType">The integration capability interface this tool implements (e.g. <c>IBSAArchiveCreator</c>)</param>
/// <param name="RegistrationConfigDefinition">Default registration settings for this integration, or null if none</param>
/// <param name="RuntimeConfigDefinition">Default runtime configuration for this integration, or null if none</param>
/// <param name="Sources">The sources this integration is available from (e.g. GitHub, Nexus Mods), or empty if none</param>
public sealed record IntegrationDescriptor(
    Guid Id,
    string DisplayName,
    string Description,
    string CategoryName,
    Type CapabilityType,
    IConfigDefinition? RegistrationConfigDefinition,
    IConfigDefinition? RuntimeConfigDefinition,
    IReadOnlyList<IIntegrationSourceDescriptor> Sources);

/// <summary>
/// Base interface for an integration: a concrete tool that can be registered and run. Specific
/// capabilities (e.g. <see cref="IArchiveCreator"/>, <see cref="ILipGenerator"/>) inherit from
/// this and add the contract for interacting with a category of external tools.
/// </summary>
public interface IIntegration {
    IntegrationDescriptor Descriptor { get; }
    static virtual IntegrationDescriptor StaticDescriptor => throw new NotImplementedException("Integration must implement Descriptor");
}

/// <summary>Result of running an integration tool</summary>
public sealed record IntegrationRunResult(bool Succeeded, string Message);
