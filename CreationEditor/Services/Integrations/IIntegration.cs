using CreationEditor.Services.Integrations.BSA;
using CreationEditor.Services.Integrations.Lip;
namespace CreationEditor.Services.Integrations;

/// <summary>
/// Base interface for an integration: a concrete tool that can be configured and run. Specific
/// capabilities (e.g. <see cref="IBSAArchiveCreator"/>, <see cref="ILipGenerator"/>) inherit from
/// this and add the contract for interacting with a category of external tools.
/// </summary>
public interface IIntegration {
    /// <summary>Stable identifier used to persist and rehydrate the integration.</summary>
    Guid Id { get; }

    /// <summary>Display name shown in the UI (e.g. "BSA Creator X").</summary>
    string DisplayName { get; }

    /// <summary>Short description shown in the UI.</summary>
    string Description { get; }

    /// <summary>Human-readable category name (e.g. "BSA Archive Creator").</summary>
    string CategoryName { get; }

    /// <summary>Concrete type of the tool's configuration model.</summary>
    Type ConfigType { get; }

    /// <summary>Creates a fresh, default configuration instance for this tool.</summary>
    object CreateDefaultConfig();

    /// <summary>
    /// Runs the tool with the given configuration. The config is the tool's own config model.
    /// </summary>
    Task<IntegrationRunResult> RunAsync(object config, CancellationToken token);
}

/// <summary>Result of running an integration tool.</summary>
public sealed record IntegrationRunResult(bool Succeeded, string Message);

