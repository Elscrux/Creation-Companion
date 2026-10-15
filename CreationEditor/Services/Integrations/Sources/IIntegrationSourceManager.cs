namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Drives the integration system's interaction with sources: checks for updates across an
/// integration's declared sources, downloads/installs updates, and registers the integration.
/// </summary>
public interface IIntegrationSourceManager {
    /// <summary>
    /// Checks the integration's declared sources for the latest version. Returns the first
    /// source's update check result, or null if the integration has no sources.
    /// </summary>
    Task<LatestVersionResult?> CheckForUpdateAsync(IntegrationDescriptor descriptor, CancellationToken token);

    /// <summary>
    /// Downloads and installs (or updates) the integration from its first available source,
    /// extracting archives and registering the integration. Returns the download result.
    /// </summary>
    Task<DownloadResult> InstallOrUpdateAsync(IntegrationDescriptor descriptor, CancellationToken token);
}
