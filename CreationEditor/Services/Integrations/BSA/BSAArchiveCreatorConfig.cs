namespace CreationEditor.Services.Integrations.BSA;

/// <summary>
/// Configuration model for a BSA archive creator tool. These are the settings needed to run the
/// tool, usable from CLI without UI.
/// </summary>
public sealed class BSAArchiveCreatorConfig {
    /// <summary>Compression level (0 = none, higher = more compression).</summary>
    public int CompressionLevel { get; set; } = 1;

    /// <summary>Whether to compress the archive contents.</summary>
    public bool Compress { get; set; } = true;

    /// <summary>Whether to include subdirectories when archiving.</summary>
    public bool IncludeSubdirectories { get; set; } = true;
}
