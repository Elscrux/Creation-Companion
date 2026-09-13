using System.IO.Abstractions;
using Serilog;
namespace CreationEditor.Services.Integrations.BSA;

/// <summary>
/// A second concrete BSA archive creator tool implementation. Demonstrates that multiple tools
/// can implement the same <see cref="IBSAArchiveCreator"/> capability and be selected from the
/// Integrations window.
/// </summary>
public sealed class AlternateBSAArchiveCreator(IFileSystem fileSystem, ILogger logger) : IBSAArchiveCreator {
    public Guid Id { get; } = new("B5A1C0DE-0002-0000-0000-000000000002");
    public string DisplayName => "Alternate BSA Archive Creator";
    public string Description => "An alternate BSA archive creator tool (dummy implementation).";
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

        logger.Here().Information(
            "Alternate BSA archive creation requested: {Source} -> {Target} (compress={Compress}, level={Level})",
            sourceFolder,
            targetArchivePath,
            bsaConfig.Compress,
            bsaConfig.CompressionLevel);

        return Task.FromResult(new IntegrationRunResult(true, "BSA archive created (alternate tool)."));
    }
}
