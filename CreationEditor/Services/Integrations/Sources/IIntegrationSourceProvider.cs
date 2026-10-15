namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Discovers all <see cref="IIntegrationSource"/> implementations registered in the DI container.
/// </summary>
public interface IIntegrationSourceProvider {
    /// <summary>All discovered sources, keyed by <see cref="IIntegrationSource.Name"/>.</summary>
    IReadOnlyDictionary<string, IIntegrationSource> Sources { get; }

    /// <summary>Gets a source by name, or null if not found.</summary>
    IIntegrationSource? GetSource(string name);
}
