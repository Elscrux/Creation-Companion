using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetController(
    IDeleteDirectoryProvider deleteDirectoryProvider,
    IAssetTypeService assetTypeService,
    IDataSourceService dataSourceService,
    IAssetReferenceController assetReferenceController,
    IModelModificationService modelModificationService,
    ILinkCacheProvider linkCacheProvider,
    IRecordController recordController,
    ILogger logger)
    : IAssetController {

    private FileSystemLink CreateDeletePath(FileSystemLink link) {
        var deletePath = link.FileSystem.Path.Combine(deleteDirectoryProvider.DeleteDirectory, link.DataRelativePath.Path);
        return new FileSystemLink(link.DataSource, deletePath);
    }

    public void Delete(FileSystemLink link, CancellationToken token = default) {
        try {
            MoveInternal(link, CreateDeletePath(link), true, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't delete {Path}: {Exception}", link, e.Message);
        }
    }

    public void Move(FileSystemLink origin, FileSystemLink destination, CancellationToken token = default) {
        try {
            MoveInternal(origin, destination, false, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't move {Path} to {Destination}: {Exception}", origin, destination, e.Message);
        }
    }

    public void Rename(FileSystemLink origin, string newName, CancellationToken token = default) {
        var directoryPath = origin.FileSystem.Path.GetDirectoryName(origin.DataRelativePath.Path);

        if (directoryPath is not null) {
            var destination = new FileSystemLink(origin.DataSource, origin.FileSystem.Path.Combine(directoryPath, newName));
            try {
                MoveInternal(origin, destination, true, token);
            } catch (Exception e) {
                logger.Here().Error(e, "Couldn't rename {Path} to {NewName}: {Exception}", origin, newName, e.Message);
            }
        } else {
            logger.Here().Warning("Couldn't find path to base directory of {Path}", origin);
        }
    }

    private void MoveInternal(FileSystemLink origin, FileSystemLink destination, bool rename, CancellationToken token) {
        var originIsFile = origin.IsFile;

        // Get the parent directory for a file path or self for a directory path
        var baseDirectory = originIsFile ? origin.ParentDirectory?.DataRelativePath.Path : origin.DataRelativePath.Path;
        if (baseDirectory is null) return;

        var baseIsFile = originIsFile || origin.FileSystem.Path.HasExtension(baseDirectory);
        var destinationIsFile = destination.IsFile;

        if (originIsFile) {
            MoveInt(origin, token);
        } else {
            foreach (var assetLink in origin.EnumerateFileLinks(true)) {
                MoveInternal(origin, assetLink, rename, token);
            }
        }

        void MoveInt(FileSystemLink fileLink, CancellationToken innerToken) {
            // Calculate the full path of that the file should be moved to
            DataRelativePath newDataRelativePath;
            if (destinationIsFile) {
                newDataRelativePath = destination.DataRelativePath;
            } else {
                var adjustedBasePath = rename || baseIsFile
                    // Example Renaming meshes\clutter to meshes\clutter-new
                    // basePath:          meshes\clutter
                    // assetFile.Path:    meshes\clutter\test.nif
                    // destination:       meshes\clutter-new\
                    // fullNewPath:       meshes\clutter-new\test.nif
                    ? destination.DataRelativePath.Path
                    // Example Moving meshes\clutter to meshes\clutter-new
                    // basePath:          meshes\clutter
                    // assetFile.Path:    meshes\clutter\test.nif
                    // destination:       meshes\clutter-new\
                    // fullNewPath:       meshes\clutter-new\clutter\test.nif
                    : destination.FileSystem.Path.Combine(destination.DataRelativePath.Path, destination.FileSystem.Path.GetFileName(baseDirectory));

                var nestedPath = DataRelativePath.PathComparer.Equals(baseDirectory, fileLink.DataRelativePath.Path)
                    ? origin.FileSystem.Path.GetFileName(baseDirectory)
                    : origin.FileSystem.Path.GetRelativePath(baseDirectory, fileLink.DataRelativePath.Path);

                newDataRelativePath = destination.FileSystem.Path.Combine(adjustedBasePath, nestedPath);
            }
            var newPath = new FileSystemLink(destination.DataSource, newDataRelativePath);

            // Reaching point of no return - after this alterations will be made 
            if (innerToken.IsCancellationRequested) return;

            // Move the asset
            if (!FileSystemMove(fileLink, newPath, innerToken)) return;

            RemapReferences(fileLink, newPath);
        }
    }

    private void RemapReferences(FileSystemLink link, FileSystemLink newLink) {
        // Remap references in records
        RemapRecordReferences(link, newLink);

        // Remap references in NIFs
        RemapAssetReferences(link, newLink);
    }

    private void RemapAssetReferences(FileSystemLink link, FileSystemLink newLink) {
        var assetLink = assetTypeService.GetAssetLink(link.DataRelativePath);
        if (assetLink is null) {
            logger.Here().Warning("Couldn't find asset type for {Path}", link.DataRelativePath);
            return;
        }

        foreach (var reference in assetReferenceController.GetAssetReferences(assetLink)) {
            if (dataSourceService.TryGetFileLink(reference, out var referenceFileLink)) {
                modelModificationService.RemapLinks(
                    referenceFileLink,
                    p => !p.IsNullOrWhitespace() && referenceFileLink.DataRelativePath.Path.EndsWith(p, DataRelativePath.PathComparison),
                    newLink.DataRelativePath.Path);
            } else {
                logger.Here().Warning("Couldn't find file link {Reference} of {Path}", reference, link.FullPath);
            }
        }
    }

    private void RemapRecordReferences(FileSystemLink link, FileSystemLink newLink) {
        var assetLink = assetTypeService.GetAssetLink(link.DataRelativePath);
        if (assetLink is null) {
            logger.Here().Warning("Couldn't find asset type for {Path}", link.DataRelativePath);
            return;
        }

        // Path without the base folder prefix
        // i.e. Change meshes\clutter\test\test.nif to clutter\test.nif
        var shortenedDataRelativePath = link.FileSystem.Path.GetRelativePath(assetLink.Type.BaseFolder, newLink.DataRelativePath.Path);

        foreach (var formLink in assetReferenceController.GetRecordReferences(assetLink)) {
            if (!linkCacheProvider.LinkCache.TryResolve(formLink, out var record)) continue;

            var recordOverride = recordController.GetOrAddOverride(record);
            recordOverride.RemapListedAssetLinks(new Dictionary<IAssetLinkGetter, string>(AssetLinkEqualityComparer.Instance) {
                { assetLink, shortenedDataRelativePath },
            });
        }
    }

    private bool FileSystemMove(FileSystemLink origin, FileSystemLink destination, CancellationToken token) {
        var destinationDirectory = destination.FileSystem.Path.GetDirectoryName(destination.FullPath);

        try {
            if (destinationDirectory is not null) destination.FileSystem.Directory.CreateDirectory(destinationDirectory);

            if (origin.Exists()) {
                if (token.IsCancellationRequested) return false;

                origin.FileSystem.File.Move(origin.FullPath, destination.FullPath);
            } else {
                if (token.IsCancellationRequested) return false;

                origin.FileSystem.Directory.Move(origin.FullPath, destination.FullPath);
            }
        } catch (Exception e) {
            logger.Here().Warning(e, "Couldn't move {File} to {Dir}: {Exception}", origin, destination, e.Message);
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
