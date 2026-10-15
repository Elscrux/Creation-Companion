using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Integrations.Sources.GitHub;
using Serilog;
namespace CreationEditor.Services.Integrations.Textures;

/// <summary>Interpolation filters for resizing.</summary>
public enum TextureInterpolationFilter {
    Default,
    POINT,
    LINEAR,
    CUBIC,
    FANT,
    BOX,
    TRIANGLE,
    POINT_DITHER,
    LINEAR_DITHER,
    CUBIC_DITHER,
    FANT_DITHER,
    BOX_DITHER,
    TRIANGLE_DITHER,
    POINT_DITHER_DIFFUSION,
    LINEAR_DITHER_DIFFUSION,
    CUBIC_DITHER_DIFFUSION,
    FANT_DITHER_DIFFUSION,
    BOX_DITHER_DIFFUSION,
    TRIANGLE_DITHER_DIFFUSION,
}

public sealed class TexConvRegistrationConfig : IConfigDefinition {
    public static FileConfigOption ToolPathOption { get; } = new(
        "TexConv Path",
        "Path to the TexConv executable");

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ToolPathOption,
    ];
}

public sealed class TexConvRuntimeConfig : IConfigDefinition {
    public static BoolConfigOption Mipmaps { get; } = new(
        "Mipmaps",
        "Generate mipmaps for textures",
        true);

    public static EnumConfigOption Filter { get; } = new(
        "Interpolation Filter",
        typeof(TextureInterpolationFilter),
        "Interpolation filter used when resizing textures",
        TextureInterpolationFilter.Default);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        Mipmaps,
        Filter,
    ];
}

/// <summary>
/// Integration for the TexConv tool from DirectXTex, to convert, compress and resize textures.
/// </summary>
public sealed class TexConv(IFileSystem fileSystem, ILogger logger, ConfigValues registrationConfigValues)
    : ITextureProcessor {
    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("5d3f2a91-8c47-4b6e-9d20-3a7f1c5e8b42"),
        "TexConv",
        "Texture converter from DirectXTex: converts, compresses (BC1/BC3/BC5/BC7), resizes and generates mipmaps for DDS and image files.",
        "Texture Processor",
        typeof(ITextureProcessor),
        new TexConvRegistrationConfig(),
        new TexConvRuntimeConfig(),
        [
            new GitHubIntegrationSourceDescriptor {
                Owner = "microsoft",
                Repo = "DirectXTex",
                AssetNamePattern = "texconv.exe",
            },
        ]
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public async Task<TextureProcessResult> ProcessTexture(
        string sourceFilePath,
        string targetFolder,
        TextureFormat? targetFormat,
        long? targetWidth,
        long? targetHeight,
        ConfigValues config,
        CancellationToken token) {
        if (!fileSystem.File.Exists(sourceFilePath)) {
            return TextureProcessResult.Fail($"Source texture not found: {sourceFilePath}");
        }

        var toolPath = registrationConfigValues.Get(TexConvRegistrationConfig.ToolPathOption);
        if (string.IsNullOrEmpty(toolPath) || !fileSystem.File.Exists(toolPath)) {
            return TextureProcessResult.Fail($"TexConv executable not found at: {toolPath}");
        }

        fileSystem.Directory.CreateDirectory(targetFolder);

        var arguments = $"\"{sourceFilePath}\" -o \"{targetFolder}\"";

        // Overwrite existing output file if any
        arguments += " --overwrite";

        // Separates alpha channel for resize/mipmap generation
        arguments += " --separate-alpha";

        if (targetFormat is not null) {
            arguments += $" -f {targetFormat}";
        }

        if (targetWidth is > 0) {
            arguments += $" -w {targetWidth.Value}";
        }

        if (targetHeight is > 0) {
            arguments += $" -h {targetHeight.Value}";
        }

        var filter = config.Get<TextureInterpolationFilter>(TexConvRuntimeConfig.Filter);
        if (filter != TextureInterpolationFilter.Default) {
            arguments += $" -if {filter}";
        }

        var mipmaps = config.Get(TexConvRuntimeConfig.Mipmaps);
        if (mipmaps) {
            // Generate all mip maps with m 0
            arguments += " -m 0";
        }

        try {
            var (exitCode, output, error) = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (exitCode != 0) {
                logger.Here().Error(
                    "TexConv failed with exit code {ExitCode}\nOutput: {Output}\nError: {Error}",
                    exitCode,
                    output,
                    error);
                return TextureProcessResult.Fail($"TexConv failed with exit code {exitCode}: {output}\n{error}");
            }

            var outputFilePath = fileSystem.Path.Combine(targetFolder, fileSystem.Path.GetFileNameWithoutExtension(sourceFilePath) + ".dds");
            if (!fileSystem.File.Exists(outputFilePath)) {
                return TextureProcessResult.Fail($"TexConv reported success but no output file was created: {outputFilePath}\n{output}");
            }

            logger.Here().Verbose("Texture processed: {Output}", outputFilePath);
            return TextureProcessResult.Ok("Texture processed.", outputFilePath);
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Texture processing was cancelled: {Source}", sourceFilePath);
            return TextureProcessResult.Fail("Texture processing was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to process texture: {Source}", sourceFilePath);
            return TextureProcessResult.Fail($"Failed to process texture: {e.Message}");
        }
    }
}
