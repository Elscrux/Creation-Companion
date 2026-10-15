using System.IO.Abstractions;
using CreationEditor.Services.Config.Options;
using Mutagen.Bethesda.Environments.DI;
using Serilog;
namespace CreationEditor.Services.Config;

/// <summary>Base directory that auto-detect path candidates are resolved against.</summary>
public enum AutoDetectBaseDirectory {
    /// <summary>The game's installation directory</summary>
    Game,

    /// <summary>The game's data directory</summary>
    Data,

    /// <summary>The editor executable's directory</summary>
    Application,
}

/// <summary>
/// A relative path candidate for auto-detecting a config option value, rooted in a base directory.
/// </summary>
/// <param name="Base">The base directory the path is rooted in</param>
/// <param name="RelativePath">Path relative to the base directory</param>
public readonly record struct AutoDetectPath(AutoDetectBaseDirectory Base, string RelativePath);

/// <summary>
/// Auto-detects file and folder config options that declare <c>AutoDetectPaths</c>: when an
/// option is not set to something else already, its candidate paths are resolved against the
/// game and data directories and the first existing match is filled in.
/// </summary>
public interface IConfigOptionAutoDetector {
    /// <summary>Fills unset file/folder options of the definition with auto-detected paths</summary>
    void AutoDetect(IConfigDefinition configDefinition, ConfigValues configValues);
}

public sealed class ConfigOptionAutoDetector(
    IFileSystem fileSystem,
    IGameDirectoryProvider gameDirectoryProvider,
    IDataDirectoryProvider dataDirectoryProvider,
    ILogger logger) : IConfigOptionAutoDetector {

    public void AutoDetect(IConfigDefinition configDefinition, ConfigValues configValues) {
        foreach (var option in configDefinition.Options) {
            var autoDetectPaths = option switch {
                FileConfigOption fileOption => fileOption.AutoDetectPaths,
                FolderConfigOption folderOption => folderOption.AutoDetectPaths,
                _ => null,
            };
            if (autoDetectPaths is null || autoDetectPaths.Count == 0) continue;

            // Only populate options that are not set to something else already
            if (configValues.TryGetValue(option.Name, out var existing)
             && existing is string existingPath
             && !string.IsNullOrWhiteSpace(existingPath)) {
                continue;
            }

            var isFolder = option is FolderConfigOption;
            foreach (var candidate in autoDetectPaths) {
                var basePath = GetBaseDirectory(candidate.Base);
                if (basePath is null) continue;

                var fullPath = fileSystem.Path.Combine(basePath, candidate.RelativePath);
                var exists = isFolder
                    ? fileSystem.Directory.Exists(fullPath)
                    : fileSystem.File.Exists(fullPath);
                if (!exists) continue;

                configValues[option.Name] = fullPath;
                logger.Here().Information("Auto-detected {Option}: {Path}", option.Name, fullPath);
                break;
            }
        }
    }

    private string? GetBaseDirectory(AutoDetectBaseDirectory baseDirectory) => baseDirectory switch {
        AutoDetectBaseDirectory.Game => TryGetPath(() => gameDirectoryProvider.Path),
        AutoDetectBaseDirectory.Data => TryGetPath(() => dataDirectoryProvider.Path),
        AutoDetectBaseDirectory.Application => AppContext.BaseDirectory,
        _ => null,
    };

    private string? TryGetPath(Func<string?> pathProvider) {
        try {
            return pathProvider();
        } catch (Exception e) {
            logger.Here().Debug(e, "Could not resolve base directory for auto-detection");
            return null;
        }
    }
}
