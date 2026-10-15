using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Integrations.Sources.GitHub;
using Serilog;
namespace CreationEditor.Services.Integrations.Textures;

public sealed class TexDiagRegistrationConfig : IConfigDefinition {
    public static FileConfigOption ToolPathOption { get; } = new(
        "TexDiag Path",
        "Path to the TexDiag executable");

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ToolPathOption,
    ];
}

/// <summary>
/// Integration for the TexDiag tool from DirectXTex, to gather standardized texture information.
/// Runs <c>texdiag info</c> and parses the reported metadata into a <see cref="TextureInfo" />.
/// </summary>
public sealed class TexDiag(IFileSystem fileSystem, ILogger logger, ConfigValues registrationConfigValues)
    : ITextureInfoProvider {
    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("c2e8b4d7-61f9-4a35-8e07-9b4c2d6f1a83"),
        "TexDiag",
        "Texture diagnostic tool from DirectXTex: reports format, dimensions, mip levels and alpha mode of DDS and image files.",
        "Texture Info Provider",
        typeof(ITextureInfoProvider),
        new TexDiagRegistrationConfig(),
        null,
        [
            new GitHubIntegrationSourceDescriptor {
                Owner = "microsoft",
                Repo = "DirectXTex",
                AssetNamePattern = "texdiag.exe",
            },
        ]
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public async Task<TextureInfoResult> GetTextureInfo(string wildcard, CancellationToken token) {
        if (string.IsNullOrEmpty(wildcard)) {
            return TextureInfoResult.Fail("No wildcard provided.");
        }

        var toolPath = registrationConfigValues.Get(TexDiagRegistrationConfig.ToolPathOption);
        if (string.IsNullOrEmpty(toolPath) || !fileSystem.File.Exists(toolPath)) {
            return TextureInfoResult.Fail($"TexDiag executable not found at: {toolPath}");
        }

        var arguments = $"info -r \"{wildcard}\"";

        try {
            var (exitCode, output, error) = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (exitCode != 0) {
                logger.Here().Error(
                    "TexDiag failed with exit code {ExitCode}\nOutput: {Output}\nError: {Error}",
                    exitCode,
                    output,
                    error);
                return TextureInfoResult.Fail($"TexDiag failed with exit code {exitCode}: {output}\n{error}");
            }

            var infos = ParseInfoOutput(output);
            if (infos.Count == 0) {
                return TextureInfoResult.Fail($"Could not parse TexDiag output for: {wildcard}\n{output}");
            }

            return TextureInfoResult.Ok(infos);
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Texture info query was cancelled: {Wildcard}", wildcard);
            return TextureInfoResult.Fail("Texture info query was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to get texture info: {Wildcard}", wildcard);
            return TextureInfoResult.Fail($"Failed to get texture info: {e.Message}");
        }
    }

    /// <summary>
    /// Parses the output of the <c>texdiag info</c> command into a dictionary of <see cref="DataSourceFileLink" /> to
    /// <see cref="TextureInfo" />, one entry per file. Note that this might also be parsing the output of a -r recursive
    /// search, in which case the output will contain multiple files.
    /// </summary>
    /// <param name="output">Raw output of the <c>texdiag info</c> command</param>
    /// <returns>Dictionary mapping each reported file to its texture info</returns>
    public Dictionary<string, TextureInfo> ParseInfoOutput(string output) {
        var results = new Dictionary<string, TextureInfo>();
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? currentLink = null;

        foreach (var rawLine in output.Split('\n')) {
            var line = rawLine.Trim('\r');
            var parts = line.Split(" = ", StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && parts[0].Length > 0) {
                values[parts[0]] = parts[1];
            } else if (fileSystem.File.Exists(line)) {
                // A line with a path starts a new file section
                FlushSection();
                currentLink = line;
            }
        }

        FlushSection();

        return results;

        void FlushSection() {
            if (currentLink is not null && TryParseInfo(values) is {} info) {
                results[currentLink] = info;
            }

            values.Clear();
            currentLink = null;
        }
    }

    private static TextureInfo? TryParseInfo(Dictionary<string, string> values) {
        if (!values.TryGetValue("width", out var widthValue)
         || !values.TryGetValue("height", out var heightValue)
         || !values.TryGetValue("format", out var format)) {
            return null;
        }

        if (!uint.TryParse(widthValue, out var width) || !uint.TryParse(heightValue, out var height)) {
            return null;
        }

        values.TryGetValue("mipLevels", out var mipValue);
        _ = int.TryParse(mipValue, out var mipCount);

        return new TextureInfo(width, height, format, mipCount);
    }
}
