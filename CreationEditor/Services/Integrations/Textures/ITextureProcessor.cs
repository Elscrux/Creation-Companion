namespace CreationEditor.Services.Integrations.Textures;

public enum TextureFormat {
    BC1_UNORM,
    BC2_UNORM,
    BC3_UNORM,
    BC4_UNORM,
    BC5_UNORM,
    BC7_UNORM,
    B8G8R8A8_UNORM,
}

/// <summary>Result of a texture processing operation.</summary>
/// <param name="Succeeded">Whether the texture was processed successfully</param>
/// <param name="Message">Details about the operation outcome</param>
/// <param name="OutputFilePath">Path of the processed texture, or null on failure</param>
public sealed record TextureProcessResult(bool Succeeded, string Message, string? OutputFilePath = null) {
    public static TextureProcessResult Ok(string message, string outputFilePath) => new(true, message, outputFilePath);
    public static TextureProcessResult Fail(string message) => new(false, message);
}

/// <summary>
/// Integration capability for processing textures (format conversion, resizing, mipmap generation).
/// Concrete tools that can convert textures implement this interface.
/// </summary>
public interface ITextureProcessor : IIntegration {
    /// <summary>
    /// Processes the texture at <paramref name="sourceFilePath" /> and writes the result into
    /// <paramref name="targetFolder" /> using the original file name (with the output format's extension).
    /// </summary>
    /// <param name="sourceFilePath">Path of the source texture</param>
    /// <param name="targetFolder">Folder the processed texture is written to</param>
    /// <param name="targetFormat">Format to convert to, or null to leave the format unchanged</param>
    /// <param name="targetWidth">Target width in pixels, or null to keep the original width</param>
    /// <param name="targetHeight">Target height in pixels, or null to keep the original height</param>
    /// <param name="config">Universal tool settings, read through the tool's own config definition</param>
    /// <param name="token">Cancellation token</param>
    Task<TextureProcessResult> ProcessTexture(
        string sourceFilePath,
        string targetFolder,
        TextureFormat? targetFormat,
        long? targetWidth,
        long? targetHeight,
        ConfigValues config,
        CancellationToken token);

    /// <summary>
    /// Converts the texture at <paramref name="sourceFilePath" /> into a DDS file with the given
    /// <paramref name="targetFormat" />. The output file is written into <paramref name="targetFolder" />
    /// using the original file name with a .dds extension.
    /// </summary>
    /// <param name="sourceFilePath">Path of the source texture</param>
    /// <param name="targetFolder">Folder the converted DDS file is written to</param>
    /// <param name="targetFormat">DDS format to convert to</param>
    /// <param name="config">Universal tool settings, read through the tool's own config definition</param>
    /// <param name="token">Cancellation token</param>
    Task<TextureProcessResult> ConvertToDds(
        string sourceFilePath,
        string targetFolder,
        TextureFormat targetFormat,
        ConfigValues config,
        CancellationToken token);
}
