using System.Reactive;
namespace CreationEditor.Services.Integrations;

/// <summary>
/// Holds the user's configured integrations (added tools with install path + config) and
/// persists them via the <c>ISetting</c> system. Emits <see cref="Changed"/> whenever the set of
/// configured integrations changes.
/// </summary>
public interface IIntegrationRegistry {
    /// <summary>All configured integrations.</summary>
    IReadOnlyList<RegisteredIntegration> Integrations { get; }

    /// <summary>Emits whenever the configured integrations change.</summary>
    IObservable<Unit> Changed { get; }

    /// <summary>Adds or updates a configured integration.</summary>
    void Save(RegisteredIntegration integration);

    /// <summary>Removes a configured integration.</summary>
    void Remove(RegisteredIntegration integration);

    /// <summary>Persists the current configured integrations to disk.</summary>
    void Persist();
}
