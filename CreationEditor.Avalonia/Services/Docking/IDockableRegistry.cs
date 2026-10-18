namespace CreationEditor.Avalonia.Services.Docking;

/// <summary>
/// Registry of all registered <see cref="IDockableDefinition"/>s. Populated from DI, including
/// definitions registered by plugin extension modules.
/// </summary>
public interface IDockableRegistry {
    /// <summary>
    /// All registered dockable definitions.
    /// </summary>
    IReadOnlyList<IDockableDefinition> Definitions { get; }

    /// <summary>
    /// Get a dockable definition by its type.
    /// </summary>
    /// <param name="type">Type of the dockable.</param>
    /// <returns>The definition, or <see langword="null"/> if no dockable is registered under that type.</returns>
    IDockableDefinition? GetByType(string type);
}
