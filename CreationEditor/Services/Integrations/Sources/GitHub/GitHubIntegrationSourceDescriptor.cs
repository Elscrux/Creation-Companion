namespace CreationEditor.Services.Integrations.Sources.GitHub;

/// <summary>
/// Strongly-typed configuration locating an integration on GitHub: owner, repo, and optional tag.
/// </summary>
public sealed class GitHubIntegrationSourceDescriptor : IIntegrationSourceDescriptor {
    public string SourceName => "GitHub";

    /// <summary>GitHub repository owner (user or organization).</summary>
    public string Owner { get; init; } = "";

    /// <summary>GitHub repository name.</summary>
    public string Repo { get; init; } = "";

    /// <summary>Optional release tag to download; empty uses the latest release.</summary>
    public string? Tag { get; init; }

    /// <summary>
    /// Optional glob pattern (e.g. "texconv*.exe") to select a specific release asset when a
    /// release contains multiple files. Empty downloads the first asset.
    /// </summary>
    public string? AssetNamePattern { get; init; }
}
