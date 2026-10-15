using System.IO.Abstractions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Integrations.Sources.NexusMods;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Serilog;
namespace CreationEditor.Services.Integrations.Archive;

public sealed class BSArchRegistrationConfig : IConfigDefinition {
    public static FileConfigOption ToolPathOption { get; } = new(
        "BSArch Path",
        "Path to the BSArch executable");

    public IReadOnlyList<IConfigOption> Options { get; } = [
        ToolPathOption,
    ];
}

/// <summary>Compression options supported by BSArch, mapped to the -z flag.</summary>
public enum BSACompressionType {
    None,
    Default,
    ZLIB,
    LZ4,
    LZ4_F,
}

public sealed class BSArchConfig : IConfigDefinition {
    public static EnumConfigOption Compression { get; } = new(
        "Compression",
        typeof(BSACompressionType),
        "Compression type to use (None disables compression, Default uses the game default)",
        BSACompressionType.Default);

    public static LongConfigOption SplitSize { get; } = new(
        "Split Size (GB)",
        "Split archives by N GB, 0 disables splitting. When not set BSA archives are split by 2 GB, BA2 aren't split. Games before Fallout 4 have hardcoded 2 GB limit!",
        2,
        0,
        isOptional: true);

    public static TextConfigOption FileFilters { get; } = new(
        "File Filters",
        "Pack files only matching the provided mask(s), separated by comma",
        isOptional: true);

    public static BoolConfigOption ShareFiles { get; } = new(
        "Share Identical Files",
        "Identical files share the same data in the archive to preserve space",
        true,
        true);

    public static BoolConfigOption Multithreaded { get; } = new(
        "Multithreaded",
        "Use available CPU cores for faster multithreaded processing",
        true,
        true);

    public static TextConfigOption ArchiveFlags { get; } = new(
        "Archive Flags Override",
        "Override archive flags with a hex value (Oblivion, Fallout 3/NV and Skyrim archives only)",
        isOptional: true);

    public static TextConfigOption FileFlags { get; } = new(
        "File Flags Override",
        "Override file flags with a hex value (Oblivion, Fallout 3/NV and Skyrim archives only)",
        isOptional: true);

    public IReadOnlyList<IConfigOption> Options { get; }

    public BSArchConfig() => Options = [
        Compression,
        SplitSize,
        FileFilters,
        ShareFiles,
        Multithreaded,
        ArchiveFlags,
        FileFlags,
    ];
}

/// <summary>
/// Integration for the BSArch tool, to create archives.
/// </summary>
public sealed class BSArch(IFileSystem fileSystem, ILogger logger, IGameReleaseContext gameReleaseContext, ConfigValues registrationConfigValues)
    : IArchiveCreator {
    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("8be01463-338c-483d-ad3f-aebbc3ce17c9"),
        "BSArch",
        "A tool for packing and unpacking bsa and ba2 archives.",
        "Archive Creator",
        typeof(IArchiveCreator),
        new BSArchRegistrationConfig(),
        new BSArchConfig(),
        [
            new NexusModsIntegrationSourceDescriptor {
                GameId = "newvegas",
                ModId = 64745,
            },
        ]
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public async Task<IntegrationRunResult> CreateArchive(
        IDataSource dataSource,
        string targetArchivePath,
        ConfigValues config,
        CancellationToken token) {
        var sourceFolder = dataSource.Path;
        if (!fileSystem.Directory.Exists(sourceFolder)) {
            return new IntegrationRunResult(false, $"Source folder not found: {sourceFolder}");
        }

        var toolPath = registrationConfigValues.Get(BSArchRegistrationConfig.ToolPathOption);
        if (string.IsNullOrEmpty(toolPath) || !fileSystem.File.Exists(toolPath)) {
            return new IntegrationRunResult(false, $"BSArch executable not found at: {toolPath}");
        }

        var compression = config.Get<BSACompressionType>(BSArchConfig.Compression);
        var splitSize = config.TryGetStruct(BSArchConfig.SplitSize);
        var fileFilters = config.TryGet(BSArchConfig.FileFilters);
        var shareFiles = config.TryGetStruct(BSArchConfig.ShareFiles);
        var multithreaded = config.TryGetStruct(BSArchConfig.Multithreaded);
        var archiveFlags = config.TryGet(BSArchConfig.ArchiveFlags);
        var fileFlags = config.TryGet(BSArchConfig.FileFlags);

        // Build the command line: BSArch.exe pack <source> <archive> [parameters]
        var arguments = $"pack \"{sourceFolder}\" \"{targetArchivePath}\"";

        // Archive format flag.
        arguments += " -" + gameReleaseContext.Release switch {
            GameRelease.Oblivion => "tes4",
            GameRelease.SkyrimLE => "tes5",
            GameRelease.SkyrimSE => "sse",
            GameRelease.SkyrimVR => "sse",
            GameRelease.EnderalLE => "tes5",
            GameRelease.EnderalSE => "sse",
            GameRelease.Fallout4 => "fo4",
            GameRelease.SkyrimSEGog => "sse",
            GameRelease.Starfield => "sf1",
            GameRelease.Fallout4VR => "fo4",
            GameRelease.EnderalSEGog => "sse",
            GameRelease.Fallout3 => "fo3",
            GameRelease.FalloutNV => "fnv",
            _ => throw new InvalidOperationException($"Unsupported game release for BSArch: {gameReleaseContext.Release}"),
        };

        // Compression flag.
        if (compression != BSACompressionType.None) {
            arguments += compression == BSACompressionType.Default
                ? " -z"
                : $" -z:{CompressionType(compression)}";
        }

        // Split archives by N GB (0 disables splitting)
        if (splitSize.HasValue) {
            arguments += $" -split:{splitSize.Value}";
        }

        // File filters.
        if (!string.IsNullOrWhiteSpace(fileFilters)) {
            arguments += $" -f:{fileFilters}";
        }

        // Identical file sharing.
        if (shareFiles.HasValue) {
            arguments += shareFiles.Value ? " -share:yes" : " -share:no";
        }

        // Multithreaded processing.
        if (multithreaded.HasValue) {
            arguments += multithreaded.Value ? " -mt:yes" : " -mt:no";
        }

        // Archive and file flag overrides.
        if (!string.IsNullOrWhiteSpace(archiveFlags)) {
            arguments += $" -af:{archiveFlags}";
        }

        if (!string.IsNullOrWhiteSpace(fileFlags)) {
            arguments += $" -ff:{fileFlags}";
        }

        logger.Here().Information(
            "Creating archive: {Source} -> {Target} (compression={Compression}, split={Split}, filters={Filters}, share={Share}, mt={Mt})",
            sourceFolder,
            targetArchivePath,
            compression,
            splitSize,
            fileFilters,
            shareFiles,
            multithreaded);

        try {
            var (exitCode, output, error) = await ProcessRunner.RunProcessAsync(toolPath, arguments, token);

            if (exitCode != 0) {
                logger.Here().Error(
                    "BSArch failed with exit code {ExitCode}\nOutput: {Output}\nError: {Error}",
                    exitCode,
                    output,
                    error);
                return new IntegrationRunResult(false, $"BSArch failed with exit code {exitCode}: {output}\n{error}");
            }

            logger.Here().Information("Archive created: {Target}", targetArchivePath);
            return new IntegrationRunResult(true, "Archive created.");
        } catch (OperationCanceledException e) {
            logger.Here().Warning(e, "Archive creation was cancelled: {Target}", targetArchivePath);
            return new IntegrationRunResult(false, "Archive creation was cancelled.");
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to create archive: {Target}", targetArchivePath);
            return new IntegrationRunResult(false, $"Failed to create archive: {e.Message}");
        }
    }

    private static string CompressionType(BSACompressionType compression) {
        return compression switch {
            BSACompressionType.ZLIB => "zlib",
            BSACompressionType.LZ4 => "lz4",
            BSACompressionType.LZ4_F => "lz4f",
            _ => throw new ArgumentOutOfRangeException(nameof(compression), compression, null),
        };
    }
}
