namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Discovers all <see cref="IIntegrationSource"/> implementations from the DI container.
/// </summary>
public sealed class IntegrationSourceProvider(IEnumerable<IIntegrationSource> sources) : IIntegrationSourceProvider {
    public IReadOnlyDictionary<string, IIntegrationSource> Sources { get; } = sources.ToDictionary(s => s.Name);

    public IIntegrationSource? GetSource(string name) => Sources.GetValueOrDefault(name);
}
