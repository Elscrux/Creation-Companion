﻿using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Serilog;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class ModSaveService(
    IEditorEnvironment editorEnvironment,
    ILinkCacheProvider linkCacheProvider,
    IAssetTypeProvider assetTypeProvider,
    IModSaveLocationProvider modSaveLocationProvider,
    IFileSystem fileSystem,
    ISavePipeline savePipeline,
    ILogger logger)
    : IModSaveService {

    public void SaveMod(IMod mod) {
        if (mod.ModKey == ModKey.Null) return;

        logger.Here().Information("Saving mod {ModName}", mod.ModKey.FileName);
        var filePath = modSaveLocationProvider.GetSaveLocation(mod);

        savePipeline.Execute(linkCacheProvider.LinkCache, mod);

        // Don't save empty mods
        if (!mod.EnumerateMajorRecords().Any()) {
            logger.Here().Information("Skipping saving empty mod {ModName}", mod.ModKey.FileName);
            return;
        }

        try {
            // Try to save mod
            Write(mod, filePath);
        } catch (Exception e) {
            logger.Here().Warning(
                e,
                "Failed to save mod {ModName} at {FilePath}, try backup location instead: {Exception}",
                mod.ModKey.FileName,
                filePath,
                e.Message);
            try {
                // Save at backup location if failed once
                filePath = modSaveLocationProvider.GetBackupSaveLocation(mod);
                Write(mod, filePath);
            } catch (Exception e2) {
                logger.Here().Warning(e2, "Failed to save mod {ModName} at {FilePath}: {Exception}", mod.ModKey.FileName, filePath, e2.Message);
            }
        }

        logger.Here().Information("Finished saving mod {ModName}", mod.ModKey.FileName);
    }

    private void Write(IMod mod, string filePath) {
        var builder = mod.BeginWrite
            .ToPath(filePath)
            .WithLoadOrder(editorEnvironment.GameEnvironment.LoadOrder)
            .WithDataFolder(editorEnvironment.GameEnvironment.DataFolderPath)
            .WithFileSystem(fileSystem)
            .WithAllParentMasters();

        // If the mod is using localization, set up the strings writer
        if (mod.UsingLocalization) {
            var stringsFolder = fileSystem.Path.Combine(modSaveLocationProvider.GetSaveLocation(), assetTypeProvider.Translation.BaseFolder);

            builder
                .WithStringsWriter(new StringsWriter(
                    editorEnvironment.GameEnvironment.GameRelease,
                    mod.ModKey,
                    stringsFolder,
                    MutagenEncoding.Default,
                    fileSystem))
                .WithTargetLanguage(Language.English);
        }

        builder.Write();
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
            logger.Here().Warning(e,
                "Failed to create backup of mod {ModName} at {FilePath}: {Exception}",
                mod.ModKey.FileName,
                filePath,
                e.Message);
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

    private string GetBackupFilePath(string backupLocation, string fileName, DateTime writeTime) =>
        fileSystem.Path.Combine(backupLocation, $"{fileName}.{GetTimeFileName(writeTime)}.bak");
    private static string GetTimeFileName(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd_HH-mm-ss");
}
