using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class ModSaveService : IModSaveService {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IModSaveLocationProvider _modSaveLocationProvider;
    private readonly IFileSystem _fileSystem;
    private readonly ISavePipeline _savePipeline;
    private readonly ILogger _logger;

    public ModSaveService(
        IEditorEnvironment editorEnvironment,
        IModSaveLocationProvider modSaveLocationProvider,
        IFileSystem fileSystem,
        ISavePipeline savePipeline,
        ILogger logger) {
        _editorEnvironment = editorEnvironment;
        _modSaveLocationProvider = modSaveLocationProvider;
        _fileSystem = fileSystem;
        _savePipeline = savePipeline;
        _logger = logger;
    }

    public void SaveMod(IMod mod) {
        if (mod.ModKey == ModKey.Null) return;

        var filePath = _modSaveLocationProvider.GetSaveLocation(mod);

        // todo add options for localization export!
        var binaryWriteParameters = new BinaryWriteParameters {
            StringsWriter = null,
            TargetLanguageOverride = null,
            Encodings = null
        };

        _savePipeline.Execute(_editorEnvironment.LinkCache, mod);

        // Don't save empty mods
        if (!mod.EnumerateMajorRecords().Any()) {
            _logger.Here().Information("Skipping saving empty mod {ModName}", mod.ModKey.FileName);
            return;
        }

        _logger.Here().Information("Saving mod {ModName}", mod.ModKey.FileName);
        try {
            // Try to save mod
            mod.WriteToBinaryParallel(filePath, binaryWriteParameters, _fileSystem);
        } catch (Exception e) {
            _logger.Here().Warning("Failed to save mod {ModName} at {FilePath}, try backup location instead: {Exception}", mod.ModKey.FileName, filePath, e.Message);
            try {
                // Save at backup location if failed once
                filePath = _modSaveLocationProvider.GetBackupSaveLocation(mod);
                mod.WriteToBinaryParallel(filePath, binaryWriteParameters, _fileSystem);
            } catch (Exception e2) {
                _logger.Here().Warning("Failed to save mod {ModName} at {FilePath}: {Exception}", mod.ModKey.FileName, filePath, e2.Message);
            }
        }
    }

    public void BackupMod(IMod mod, int limit = -1) {
        if (mod.ModKey == ModKey.Null) return;

        var filePath = _fileSystem.FileInfo.New(_modSaveLocationProvider.GetSaveLocation(mod));

        if (!filePath.Exists) return;

        try {
            var backupLocation = _modSaveLocationProvider.GetBackupSaveLocation();
            var backupFilePath = GetBackupFilePath(backupLocation, filePath.Name, filePath.LastWriteTime);
            _fileSystem.Directory.CreateDirectory(backupLocation);
            _fileSystem.File.Copy(
                filePath.FullName,
                backupFilePath,
                true);
        } catch (Exception e) {
            _logger.Here().Warning(
                "Failed to create backup of mod {ModName} at {FilePath}: {Exception}", mod.ModKey.FileName, filePath, e.Message);
        }

        LimitBackups(limit, mod);
    }

    private void LimitBackups(int limit, IModGetter mod) {
        if (limit <= 0) return;

        var backupSaveLocation = _modSaveLocationProvider.GetBackupSaveLocation();
        if (!_fileSystem.Directory.Exists(backupSaveLocation)) return;

        var backupNameStart = $"{mod.ModKey.FileName}.*.bak";
        var backupFiles = _fileSystem.Directory
            .EnumerateFiles(backupSaveLocation, backupNameStart)
            .ToList();

        if (backupFiles.Count > limit) {
            var filesToDelete = backupFiles
                .OrderByDescending(fileName => fileName)
                .Skip(limit)
                .ToList();

            foreach (var deleteFile in filesToDelete) {
                _fileSystem.File.Delete(deleteFile);
            }
        }
    }

    private string GetBackupFilePath(string backupLocation, string fileName, DateTime writeTime) => _fileSystem.Path.Combine(backupLocation, $"{fileName}.{GetTimeFileName(writeTime)}.bak");
    public string GetTimeFileName(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd_HH-mm-ss");
}
