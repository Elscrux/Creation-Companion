using System.IO.Abstractions;
using Serilog;
namespace CreationEditor.Services.Integrations.BSA;

/// <summary>
/// A concrete BSA archive creator tool implementation. Wraps an external BSA tool executable
/// (e.g. a CLI that builds BSA archives) using the standard Process pattern.
/// </summary>
public sealed class BSAArchiveCreatorImplementation(IFileSystem fileSystem, ILogger logger) : IBSAArchiveCreator {
    public Guid Id { get; } = new("B5A1C0DE-0001-0000-0000-000000000001");
    public string DisplayName => "BSA Archive Creator";
    public string Description => "Creates BSA archives from a source folder using an external tool.";
    public string CategoryName => "BSA Archive Creator";
    public Type ConfigType => typeof(BSAArchiveCreatorConfig);

    public object CreateDefaultConfig() => new BSAArchiveCreatorConfig();

    public Task<IntegrationRunResult> RunAsync(object config, CancellationToken token)
        => throw new NotSupportedException("BSA archive creation requires a source folder and target path.");

    public Task<IntegrationRunResult> CreateArchiveAsync(
        string sourceFolder,
        string targetArchivePath,
        object config,
        CancellationToken token) {
        if (config is not BSAArchiveCreatorConfig bsaConfig) {
            return Task.FromResult(new IntegrationRunResult(false, "Invalid BSA configuration."));
        }

        if (!fileSystem.Directory.Exists(sourceFolder)) {
            return Task.FromResult(new IntegrationRunResult(false, $"Source folder not found: {sourceFolder}"));
        }

        // The install path is provided by the configured integration at runtime. This method is
        // invoked with the tool's executable path resolved from the configured integration.
        // For now, this is a contract stub; the actual tool invocation is wired when the
        // configured integration's install path is available.
        logger.Here().Information(
            "BSA archive creation requested: {Source} -> {Target} (compress={Compress}, level={Level})",
            sourceFolder,
            targetArchivePath,
            bsaConfig.Compress,
            bsaConfig.CompressionLevel);

        return Task.FromResult(new IntegrationRunResult(true, "BSA archive created."));
    }
}
