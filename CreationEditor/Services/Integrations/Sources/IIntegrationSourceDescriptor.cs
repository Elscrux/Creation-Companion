namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Marker interface for strongly-typed source descriptors.
/// Each integration source defines its own descriptor type, which is passed to the source's methods
/// to identify the specific repository/mod to query.
/// </summary>
public interface IIntegrationSourceDescriptor {
    /// <summary>
    /// The name of the integration source.
    /// </summary>
    string SourceName { get; }
}
