namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Result of checking a source for updates. The source reports the latest version it knows about;
/// the caller (the integration source manager) compares it against the installed version.
/// </summary>
/// <param name="LatestVersion">The latest version available from the source, or null if unknown</param>
/// <param name="Message">Optional human-readable message (e.g. error or info)</param>
public sealed record LatestVersionResult(string? LatestVersion, string? Message);

/// <summary>
/// Result of a download attempt from a source.
/// </summary>
public abstract record DownloadResult {
    /// <summary>The file was downloaded successfully to <see cref="InstalledPath"/>.</summary>
    public sealed record Succeeded(string InstalledPath) : DownloadResult;

    /// <summary>The download failed with an error message.</summary>
    public sealed record Failed(string Message) : DownloadResult;

    /// <summary>
    /// The source cannot download directly (e.g. Nexus non-premium). The user must open
    /// <see cref="BrowserUrl"/> to download manually; the app watches for <see cref="ExpectedFileName"/>.
    /// </summary>
    public sealed record ManualDownloadRequired(string BrowserUrl, string ExpectedFileName) : DownloadResult;
}

/// <summary>
/// A transport that queries a single external source (GitHub, Nexus Mods, etc.) for an integration's
/// latest version and downloads its files. Sources are discovered via DI and referenced by name from
/// an integration's declared <see cref="IIntegrationSourceDescriptor"/>s. Each source is a general
/// service: the specific repository/mod to query is supplied per-operation via the strongly-typed <see cref="IIntegrationSourceDescriptor"/>.
/// </summary>
public interface IIntegrationSource {
    /// <summary>Unique name of this source, matching <see cref="IIntegrationSourceDescriptor.SourceName"/>.</summary>
    string Name { get; }

    /// <summary>Queries the source for the latest version of the integration described by <paramref name="sourceDescriptor"/>.</summary>
    Task<LatestVersionResult> GetLatestVersionAsync(IIntegrationSourceDescriptor sourceDescriptor, CancellationToken token);

    /// <summary>Downloads the integration described by <paramref name="sourceDescriptor"/> to <paramref name="destinationPath"/>.</summary>
    Task<DownloadResult> DownloadAsync(IIntegrationSourceDescriptor sourceDescriptor, string version, string destinationPath, CancellationToken token);
}
