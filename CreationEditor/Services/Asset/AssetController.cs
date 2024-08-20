using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetController(
    IFileSystem fileSystem,
    IDataDirectoryProvider dataDirectoryProvider,
    IDeleteDirectoryProvider deleteDirectoryProvider,
    IAssetProvider assetProvider,
    IModelModificationService modelModificationService,
    ILinkCacheProvider linkCacheProvider,
    IRecordController recordController,
    ILogger logger)
    : IAssetController {

    private string CreateDeletePath(string path) {
        var relativePath = fileSystem.Path.GetRelativePath(dataDirectoryProvider.Path, path);

        return fileSystem.Path.Combine(deleteDirectoryProvider.DeleteDirectory, relativePath);
    }

    public void Delete(string path, CancellationToken token = default) {
        try {
            MoveInternal(path, CreateDeletePath(path), true, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't delete {Path}: {Exception}", path, e.Message);
        }
    }

    public void Move(string path, string destination, CancellationToken token = default) {
        try {
            MoveInternal(path, destination, false, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't move {Path} to {Destination}: {Exception}", path, destination, e.Message);
        }
    }

    public void Rename(string path, string newName, CancellationToken token = default) {
        var directoryPath = fileSystem.Path.GetDirectoryName(path);
        if (directoryPath is not null) {
            try {
                MoveInternal(path, fileSystem.Path.Combine(directoryPath, newName), true, token);
            } catch (Exception e) {
                logger.Here().Error(e, "Couldn't rename {Path} to {NewName}: {Exception}", path, newName, e.Message);
            }
        } else {
            logger.Here().Warning("Couldn't find path to base directory of {Path}", path);
        }
    }

    private void MoveInternal(string path, string destination, bool rename, CancellationToken token) {
        var isFile = fileSystem.Path.HasExtension(path);

        // Get the parent directory for a file path or self for a directory path
        var baseDirectory = isFile ? fileSystem.Path.GetDirectoryName(path) : path;
        if (baseDirectory is null) return;

        var assetContainer = assetProvider.GetAssetContainer(baseDirectory, token);

        var basePath = assetContainer.Path;
        var baseIsFile = isFile || fileSystem.Path.HasExtension(basePath);
        var destinationIsFile = fileSystem.Path.HasExtension(destination);

        if (isFile) {
            var assetFile = assetContainer.GetAssetFile(path);
            if (assetFile is null) return;

            MoveInt(assetFile, token);
        } else {
            foreach (var assetFile in assetContainer.GetAllChildren<IAsset, AssetFile>(a => a.Children, true)) {
                MoveInt(assetFile, token);
            }
        }

        void MoveInt(AssetFile assetFile, CancellationToken innerToken) {
            // Calculate the full path of that the file should be moved to
            string fullNewPath;
            if (destinationIsFile) {
                fullNewPath = destination;
            } else {
                fullNewPath = fileSystem.Path.Combine(
                    rename || baseIsFile
                        // Example Renaming meshes\clutter to meshes\clutter-new
                        // basePath:          meshes\clutter
                        // assetFile.Path:    meshes\clutter\test.nif
                        // destination:       meshes\clutter-new\
                        // fullNewPath:       meshes\clutter-new\test.nif
                        ? destination
                        // Example Moving meshes\clutter to meshes\clutter-new
                        // basePath:          meshes\clutter
                        // assetFile.Path:    meshes\clutter\test.nif
                        // destination:       meshes\clutter-new\
                        // fullNewPath:       meshes\clutter-new\clutter\test.nif
                        : fileSystem.Path.Combine(destination, fileSystem.Path.GetFileName(basePath)),
                    string.Equals(basePath, assetFile.Path, AssetCompare.PathComparison)
                        ? fileSystem.Path.GetFileName(basePath)
                        : fileSystem.Path.GetRelativePath(basePath, assetFile.Path));
            }

            var dataRelativePath = fileSystem.Path.GetRelativePath(dataDirectoryProvider.Path, fullNewPath);

            // Path without the base folder prefix
            // Change meshes\clutter\test\test.nif to clutter\test.nif
            var shortenedPath = fileSystem.Path.GetRelativePath(assetFile.ReferencedAsset.AssetLink.Type.BaseFolder, dataRelativePath);

            // Reaching point of no return - after this alterations will be made 
            if (innerToken.IsCancellationRequested) return;

            // Move the asset
            if (!FileSystemMove(assetFile.Path, fullNewPath, innerToken)) return;

            // Remap references in records
            foreach (var formLink in assetFile.ReferencedAsset.RecordReferences) {
                if (!linkCacheProvider.LinkCache.TryResolve(formLink, out var record)) continue;

                var recordOverride = recordController.GetOrAddOverride(record);
                recordOverride.RemapListedAssetLinks(new Dictionary<IAssetLinkGetter, string>(AssetLinkEqualityComparer.Instance) {
                    { assetFile.ReferencedAsset.AssetLink, shortenedPath },
                });
            }

            // Remap references in NIFs
            foreach (var reference in assetFile.ReferencedAsset.NifReferences) {
                var fullPath = fileSystem.Path.Combine(dataDirectoryProvider.Path, reference.Path);
                modelModificationService.RemapLinks(fullPath,
                    p => !p.IsNullOrWhitespace() && assetFile.Path.EndsWith(p, AssetCompare.PathComparison),
                    dataRelativePath);
            }
        }
    }

    private bool FileSystemMove(string path, string destination, CancellationToken token) {
        var destinationDirectory = fileSystem.Path.GetDirectoryName(destination);

        try {
            if (destinationDirectory is not null) fileSystem.Directory.CreateDirectory(destinationDirectory);

            if (fileSystem.File.Exists(path)) {
                if (token.IsCancellationRequested) return false;

                fileSystem.File.Move(path, destination);
            } else {
                if (token.IsCancellationRequested) return false;

                fileSystem.Directory.Move(path, destination);
            }
        } catch (Exception e) {
            logger.Here().Warning(e, "Couldn't move {File} to {Dir}: {Exception}", path, destination, e.Message);
            return false;
        }

        return true;
    }

    public void Redo() {
        throw new NotImplementedException();
    }

    public void Undo() {
        throw new NotImplementedException();
    }
}
