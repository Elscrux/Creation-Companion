namespace CreationEditor.Services.Integrations.Sources.NexusMods;

/// <summary>
/// Strongly-typed configuration locating an integration on Nexus Mods: game id and mod id.
/// </summary>
public sealed class NexusModsIntegrationSourceDescriptor : IIntegrationSourceDescriptor {
    public string SourceName => "Nexus Mods";

    /// <summary>Nexus Mods game id.</summary>
    public required string GameId { get; init; }

    /// <summary>Nexus Mods mod id.</summary>
    public required long ModId { get; init; }
}
