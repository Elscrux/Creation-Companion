using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class ModSaveService(
    ILinkCacheProvider linkCacheProvider,
    IModSaveLocationProvider modSaveLocationProvider,
    IFileSystem fileSystem,
    ISavePipeline savePipeline,
    ILogger logger)
    : IModSaveService {

    public void SaveMod(IMod mod) {
        if (mod.ModKey == ModKey.Null) return;

        var filePath = modSaveLocationProvider.GetSaveLocation(mod);

        // todo add options for localization export!
        var binaryWriteParameters = new BinaryWriteParameters {
            StringsWriter = null,
            TargetLanguageOverride = null,
            Encodings = null
        };

        savePipeline.Execute(linkCacheProvider.LinkCache, mod);

        // Don't save empty mods
        if (!mod.EnumerateMajorRecords().Any()) {
            logger.Here().Verbose("Skipping saving empty mod {ModName}", mod.ModKey.FileName);
            return;
        }

        logger.Here().Information("Saving mod {ModName}", mod.ModKey.FileName);
        try {
            // Try to save mod
            mod.WriteToBinaryParallel(filePath, binaryWriteParameters, fileSystem);
        } catch (Exception e) {
            logger.Here().Warning("Failed to save mod {ModName} at {FilePath}, try backup location instead: {Exception}", mod.ModKey.FileName, filePath, e.Message);
            try {
                // Save at backup location if failed once
                filePath = modSaveLocationProvider.GetBackupSaveLocation(mod);
                mod.WriteToBinaryParallel(filePath, binaryWriteParameters, fileSystem);
            } catch (Exception e2) {
                logger.Here().Warning("Failed to save mod {ModName} at {FilePath}: {Exception}", mod.ModKey.FileName, filePath, e2.Message);
            }
        }
    }

    public void BackupMod(IMod mod, int limit = -1) {
        if (mod.ModKey == ModKey.Null) return;

        var filePath = fileSystem.FileInfo.New(modSaveLocationProvider.GetSaveLocation(mod));

        if (!filePath.Exists) return;

        try {
            var backupLocation = modSaveLocationProvider.GetBackupSaveLocation();
            var backupFilePath = GetBackupFilePath(backupLocation, filePath.Name, filePath.LastWriteTime);
            fileSystem.Directory.CreateDirectory(backupLocation);
            fileSystem.File.Copy(
                filePath.FullName,
                backupFilePath,
                true);
        } catch (Exception e) {
            logger.Here().Warning(
                "Failed to create backup of mod {ModName} at {FilePath}: {Exception}", mod.ModKey.FileName, filePath, e.Message);
        }

        LimitBackups(limit, mod);
    }

    private void LimitBackups(int limit, IModGetter mod) {
        if (limit <= 0) return;

        var backupSaveLocation = modSaveLocationProvider.GetBackupSaveLocation();
        if (!fileSystem.Directory.Exists(backupSaveLocation)) return;

        var backupNameStart = $"{mod.ModKey.FileName}.*.bak";
        var backupFiles = fileSystem.Directory
            .EnumerateFiles(backupSaveLocation, backupNameStart)
            .ToList();

        if (backupFiles.Count > limit) {
            var filesToDelete = backupFiles
                .OrderByDescending(fileName => fileName)
                .Skip(limit)
                .ToList();

            foreach (var deleteFile in filesToDelete) {
                fileSystem.File.Delete(deleteFile);
            }
        }
    }

    private string GetBackupFilePath(string backupLocation, string fileName, DateTime writeTime) => fileSystem.Path.Combine(backupLocation, $"{fileName}.{GetTimeFileName(writeTime)}.bak");
    private static string GetTimeFileName(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd_HH-mm-ss");
}
