using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using Serilog;
namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Drives the integration system's interaction with sources: checks for updates across an
/// integration's declared sources, downloads/installs updates, extracts archives, and registers
/// the integration. Installs to <c>&lt;exeDir&gt;/tools/&lt;integrationName&gt;/</c>.
/// </summary>
public sealed class IntegrationSourceManager(
    IFileSystem fileSystem,
    IIntegrationSourceProvider sourceProvider,
    IIntegrationRegistry registry,
    IConfigOptionAutoDetector autoDetector,
    ManualDownloadHandler manualDownloadHandler,
    ILogger logger) : IIntegrationSourceManager {
    private const string ToolsFolderName = "Tools";

    public async Task<LatestVersionResult?> CheckForUpdateAsync(IntegrationDescriptor descriptor, CancellationToken token) {
        foreach (var sourceDefinition in descriptor.Sources) {
            var source = sourceProvider.GetSource(sourceDefinition.SourceName);
            if (source is null) {
                logger.Here().Warning("Integration {Integration} declares unknown source {Source}", descriptor.DisplayName, sourceDefinition.SourceName);
                continue;
            }

            try {
                return await source.GetLatestVersionAsync(sourceDefinition, token);
            } catch (Exception e) {
                logger.Here().Error(e, "Failed to check for updates on source {Source} for integration {Integration}", source.Name, descriptor.DisplayName);
            }
        }

        return null;
    }

    public async Task<DownloadResult> InstallOrUpdateAsync(IntegrationDescriptor descriptor, CancellationToken token) {
        foreach (var sourceDefinition in descriptor.Sources) {
            var source = sourceProvider.GetSource(sourceDefinition.SourceName);
            if (source is null) {
                logger.Here().Warning("Integration {Integration} declares unknown source {Source}", descriptor.DisplayName, sourceDefinition.SourceName);
                continue;
            }

            var installFolder = GetInstallFolder(descriptor);
            fileSystem.Directory.CreateDirectory(installFolder);

            var version = await source.GetLatestVersionAsync(sourceDefinition, token);
            if (version.LatestVersion is null) {
                logger.Here().Warning("Source {Source} for integration {Integration} has no latest version: {Message}",
                    source.Name,
                    descriptor.DisplayName,
                    version.Message);
                continue;
            }

            var downloadResult = await source.DownloadAsync(sourceDefinition, version.LatestVersion, installFolder, token);
            switch (downloadResult) {
                case DownloadResult.Succeeded succeeded:
                    return CompleteInstall(descriptor, sourceDefinition, succeeded.InstalledPath);
                case DownloadResult.Failed failed:
                    logger.Here().Error("Download failed for integration {Integration}: {Message}", descriptor.DisplayName, failed.Message);
                    return failed;
                case DownloadResult.ManualDownloadRequired manual:
                    // Open the browser and poll the download folder for the expected file.
                    var downloadedPath = await manualDownloadHandler.WaitForManualDownloadAsync(
                        manual.BrowserUrl,
                        manual.ExpectedFileName,
                        token: token);
                    if (downloadedPath is null) {
                        return new DownloadResult.Failed(
                            $"Manual download timed out. Please download '{manual.ExpectedFileName}' from {manual.BrowserUrl} and place it in the tools folder.");
                    }
                    return CompleteInstall(descriptor, sourceDefinition, downloadedPath);
            }
        }

        return new DownloadResult.Failed($"Integration '{descriptor.DisplayName}' has no usable sources.");
    }

    private DownloadResult CompleteInstall(IntegrationDescriptor descriptor, IIntegrationSourceDescriptor sourceDescriptor, string downloadedPath) {
        var installFolder = GetInstallFolder(descriptor);

        // Extract archives automatically and locate the executable.
        var extractor = new ArchiveExtractor(fileSystem);
        var executablePath = downloadedPath;
        if (extractor.IsArchive(downloadedPath)) {
            executablePath = extractor.Extract(downloadedPath, installFolder);
        }

        if (executablePath is null) {
            return new DownloadResult.Failed($"Could not locate an executable for integration '{descriptor.DisplayName}' after extraction.");
        }

        // Write the executable path into the registration config (e.g. the ToolPath option).
        var registrationConfig = new ConfigValues();
        var toolPathOption = descriptor.RegistrationConfigDefinition?.Options
            .OfType<FileConfigOption>()
            .FirstOrDefault();
        if (toolPathOption is not null) {
            registrationConfig.Set(toolPathOption, executablePath);
        }

        // Auto-detect any remaining file/folder configs like inside the game directory
        if (descriptor.RegistrationConfigDefinition is not null) {
            autoDetector.AutoDetect(descriptor.RegistrationConfigDefinition, registrationConfig);
        }

        // Register (or update) the integration with its source info.
        registry.Register(new IntegrationRegistration {
            IntegrationId = descriptor.Id,
            RegistrationConfig = registrationConfig,
            SourceName = sourceDescriptor.SourceName,
            InstalledVersion = null, // set after a successful update check
        });

        logger.Here().Information("Installed integration {Integration} from {Source} to {Path}", descriptor.DisplayName, sourceDescriptor.SourceName, executablePath);
        return new DownloadResult.Succeeded(executablePath);
    }

    private string GetInstallFolder(IntegrationDescriptor descriptor) {
        var exeDir = AppContext.BaseDirectory;
        return fileSystem.Path.Combine(exeDir, ToolsFolderName, descriptor.DisplayName);
    }
}
