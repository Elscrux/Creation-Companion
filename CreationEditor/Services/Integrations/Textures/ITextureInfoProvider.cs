namespace CreationEditor.Services.Integrations.Textures;

/// <summary>
/// Standardized metadata about a texture file, gathered by an <see cref="ITextureInfoProvider" />.
/// </summary>
/// <param name="Width">Width in pixels</param>
/// <param name="Height">Height in pixels</param>
/// <param name="FormatName">Format name as reported by the tool (e.g. "BC7_UNORM")</param>
/// <param name="MipCount">Number of mipmaps in the file</param>
public readonly record struct TextureInfo(
    uint Width,
    uint Height,
    string FormatName,
    int MipCount);

/// <summary>Result of a texture info query.</summary>
/// <param name="Succeeded">Whether the info could be gathered</param>
/// <param name="Infos">The gathered info per file link, or null on failure</param>
/// <param name="Message">Error message on failure</param>
public sealed record TextureInfoResult(bool Succeeded, Dictionary<string, TextureInfo>? Infos, string? Message = null) {
    public static TextureInfoResult Ok(Dictionary<string, TextureInfo> infos) => new(true, infos);
    public static TextureInfoResult Fail(string message) => new(false, null, message);
}

/// <summary>
/// Integration capability for gathering standardized metadata about texture files.
/// Concrete tools that can report texture information implement this interface.
/// </summary>
public interface ITextureInfoProvider : IIntegration {
    /// <summary>
    /// Gathers standardized metadata about the textures matching the given wildcard within the data source.
    /// </summary>
    /// <param name="wildcard">Wildcard path pattern of the textures to query</param>
    /// <param name="token">Cancellation token</param>
    Task<TextureInfoResult> GetTextureInfo(string wildcard, CancellationToken token);
}
