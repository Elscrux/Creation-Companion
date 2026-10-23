using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.DataSource;
using Serilog;
namespace CreationEditor.Services.Integrations.Lip;

public sealed class LipFuzerWrapperRegistrationConfig : IConfigDefinition {
    public static FileConfigOption ToolPathOption { get; } = new(
        "LipFuzer Path",
        "Path to the LIPFuzer executable",
        autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Game, @"Tools\LipGen\LipFuzer\LIPFuzer.exe")]);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ToolPathOption,
    ];
}

public sealed class LipFuzerWrapperRuntimeRuntimeConfig : ILipFuzerRuntimeConfigDefinition {
    public static TextConfigOption AudioExtensionOption { get; } = new(
        "Audio Extension",
        "The audio file extension to package into fuz files",
        ".xwm",
        true);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        AudioExtensionOption,
    ];
}

/// <summary>
/// Integration for the LIPFuzer tool, to pack lip and audio files into fuz files.
/// </summary>
public sealed class LipFuzerWrapper(IFileSystem fileSystem, ILogger logger, ConfigValues registrationConfigValues) : ILipFuzer {
    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("7e8c319f-ab08-40e8-947f-79989c7e9a52"),
        "LipFuzer",
        "A tool for packing lip and audio files into fuz files.",
        "Lip Fuzer",
        typeof(ILipFuzer),
        new LipFuzerWrapperRegistrationConfig(),
        new LipFuzerWrapperRuntimeRuntimeConfig(),
        []
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public string LipFileExtension => ".lip";
    public string FuzeFileExtension => ".fuz";

    public Task<IntegrationRunResult> PackageFuz(
        string dir,
        ConfigValues config,
        CancellationToken token) => PackageFuz(dir, string.Empty, config, token);

    public Task<IntegrationRunResult> PackageFuz(DataSourceFileLink file, ConfigValues config, CancellationToken token) {
        var dir = file.ParentDirectory;
        if (dir is null) {
            return Task.FromResult(new IntegrationRunResult(false, $"File {file.FullPath} does not have a parent directory."));
        }

        return PackageFuz(dir.FullPath, $"--norec -i {file.Name}", config, token);
    }

    public async Task<IntegrationRunResult> PackageFuz(
        string dir,
        string extraArgs,
        ConfigValues config,
        CancellationToken token) {
        var toolPath = registrationConfigValues.Get(LipFuzerWrapperRegistrationConfig.ToolPathOption);
        if (string.IsNullOrEmpty(toolPath) || !fileSystem.File.Exists(toolPath)) {
            return new IntegrationRunResult(false, $"LipFuzer executable not found at: {toolPath}");
        }

        var arguments = $"-s \"{dir}\" -d \"{dir}\" {extraArgs}";

        var audioExt = config.TryGet(LipFuzerWrapperRuntimeRuntimeConfig.AudioExtensionOption);
        if (!string.IsNullOrEmpty(audioExt)) {
            arguments += $" -a {audioExt}";
        }

        try {
            var (exitCode, output, error) = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (exitCode != 0) {
                logger.Here().Warning("LipFuzer failed with exit code {ExitCode}:\n{Output}\n{Error}", exitCode, output, error);
                return new IntegrationRunResult(false, $"LipFuzer failed with exit code {exitCode}: {error}");
            }

            if (output.Length > 0) {
                logger.Here().Information("LipFuzer output:\n{Output}\n{Error}", output, error);
            } else {
                logger.Here().Debug("Generated fuz files from {Dir} with audio extension {AudioExt}", dir, audioExt);
            }

            return new IntegrationRunResult(true, "Generated fuz files.");
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Lip fuzing was cancelled: {Dir}", dir);
            return new IntegrationRunResult(false, "Lip fuzing was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to fuze lips files: {Dir}", dir);
            return new IntegrationRunResult(false, $"Failed to fuze lips files: {e.Message}");
        }
    }

    public Task<IntegrationRunResult> ExtractFuz(string dir, ConfigValues config, CancellationToken token) => ExtractFuz(dir, string.Empty, config, token);

    public Task<IntegrationRunResult> ExtractFuz(DataSourceFileLink file, ConfigValues config, CancellationToken token) {
        var dir = file.ParentDirectory;
        if (dir is null) {
            return Task.FromResult(new IntegrationRunResult(false, $"File {file.FullPath} does not have a parent directory."));
        }

        return ExtractFuz(dir.FullPath, $"--norec -i {file.Name}", config, token);
    }

    public async Task<IntegrationRunResult> ExtractFuz(string dir, string extraArgs, ConfigValues config, CancellationToken token) {
        var toolPath = registrationConfigValues.Get(LipFuzerWrapperRegistrationConfig.ToolPathOption);
        if (string.IsNullOrEmpty(toolPath) || !fileSystem.File.Exists(toolPath)) {
            return new IntegrationRunResult(false, $"LipFuzer executable not found at: {toolPath}");
        }

        var arguments = $"-s \"{dir}\" -d \"{dir}\" -u {extraArgs}";

        try {
            var (exitCode, output, error) = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (exitCode != 0) {
                logger.Here().Warning("LipFuzer failed with exit code {ExitCode}:\n{Output}\n{Error}", exitCode, output, error);
                return new IntegrationRunResult(false, $"LipFuzer failed with exit code {exitCode}: {error}");
            }

            if (output.Length > 0) {
                logger.Here().Information("LipFuzer output:\n{Output}\n{Error}", output, error);
            } else {
                logger.Here().Debug("Extracted fuz files from {Dir}", dir);
            }

            return new IntegrationRunResult(true, "Extracted fuz files.");
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Fuz extraction was cancelled: {Dir}", dir);
            return new IntegrationRunResult(false, "Fuz extraction was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to extract fuz files: {Dir}", dir);
            return new IntegrationRunResult(false, $"Failed to extract fuz files: {e.Message}");
        }
    }

}
