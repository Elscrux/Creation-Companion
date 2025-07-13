using System.Diagnostics;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetController(
    IDeleteDirectoryProvider deleteDirectoryProvider,
    IAssetTypeService assetTypeService,
    IDataSourceService dataSourceService,
    IReferenceService referenceService,
    IModelModificationService modelModificationService,
    ILinkCacheProvider linkCacheProvider,
    IRecordController recordController,
    ILogger logger)
    : IAssetController {

    private FileSystemLink CreateDeletePath(FileSystemLink link) {
        var deletePath = link.FileSystem.Path.Combine(deleteDirectoryProvider.DeleteDirectory, link.DataRelativePath.Path);
        return new FileSystemLink(link.DataSource, deletePath);
    }

    public void Open(FileSystemLink link) {
        if (!link.Exists()) return;

        // Open the file via the standard program
        Process.Start(new ProcessStartInfo {
            FileName = link.FullPath,
            WorkingDirectory = link.ParentDirectory?.FullPath,
            UseShellExecute = true,
            Verb = "open",
        });
    }

    public void Delete(FileSystemLink link, CancellationToken token = default) {
        try {
            MoveInternal(link, CreateDeletePath(link), true, false, false, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't delete {Path}: {Exception}", link, e.Message);
        }
    }

    public void Copy(FileSystemLink origin, FileSystemLink destination, CancellationToken token = default) {
        try {
            MoveInternal(origin, destination, false, true, true, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't move {Path} to {Destination}: {Exception}", origin, destination, e.Message);
        }
    }

    public void Move(FileSystemLink origin, FileSystemLink destination, CancellationToken token = default) {
        try {
            MoveInternal(origin, destination, false, true, false, token);
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't move {Path} to {Destination}: {Exception}", origin, destination, e.Message);
        }
    }

    public void Rename(FileSystemLink origin, string newName, CancellationToken token = default) {
        var directoryPath = origin.FileSystem.Path.GetDirectoryName(origin.DataRelativePath.Path);

        if (directoryPath is not null) {
            var destination = new FileSystemLink(origin.DataSource, origin.FileSystem.Path.Combine(directoryPath, newName));
            try {
                MoveInternal(origin, destination, true, true, false, token);
            } catch (Exception e) {
                logger.Here().Error(e, "Couldn't rename {Path} to {NewName}: {Exception}", origin, newName, e.Message);
            }
        } else {
            logger.Here().Warning("Couldn't find path to base directory of {Path}", origin);
        }
    }

    private void MoveInternal(FileSystemLink origin, FileSystemLink destination, bool rename, bool doRemap, bool copy, CancellationToken token) {
        if (!origin.Exists()) {
            logger.Here().Warning("Cannot move {Path} because it does not exist", origin);
            return;
        }

        // Get the parent directory for a file path or self for a directory path
        var originIsFile = origin.IsFile;
        var baseDirectory = originIsFile ? origin.ParentDirectory?.DataRelativePath.Path : origin.DataRelativePath.Path;
        if (baseDirectory is null) {
            logger.Here().Warning("Cannot move {Path} because no parent directory could be found", origin);
            return;
        }

        var baseIsFile = originIsFile || origin.FileSystem.Path.HasExtension(baseDirectory);
        var destDirExists = destination.FileSystem.Directory.Exists(destination.FullPath);
        var destFileExists = destination.FileSystem.File.Exists(destination.FullPath);
        bool destinationIsFile;
        if (destFileExists) {
            destinationIsFile = true;
        } else if (destDirExists) {
            // If the destination is a directory, we need to move the file into it
            destinationIsFile = false;
        } else {
            // If the destination doesn't exist, we assume it's a directory
            destinationIsFile = destination.FileSystem.Path.HasExtension(destination.DataRelativePath.Path);
        }

        if (originIsFile) {
            MoveFile(origin, token);
        } else {
            MoveDirectory(origin, token);
        }

        void MoveFile(FileSystemLink fileLink, CancellationToken innerToken) {
            // Calculate the full path of that the file should be moved to
            DataRelativePath newDataRelativePath;
            if (destinationIsFile) {
                newDataRelativePath = destination.DataRelativePath;
            } else {
                var adjustedBasePath = rename || baseIsFile
                    // Example Renaming meshes\clutter to meshes\clutter-new
                    // baseDirectory:     meshes\clutter
                    // fileLink:          meshes\clutter\test.nif
                    // destination:       meshes\clutter-new\
                    // fullNewPath:       meshes\clutter-new\test.nif
                    ? destination.DataRelativePath.Path
                    // Example Moving meshes\clutter to meshes\clutter-new
                    // baseDirectory:     meshes\clutter
                    // fileLink:          meshes\clutter\test.nif
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
            if (!FileSystemMove(fileLink, newPath, copy, innerToken)) return;

            if (doRemap) {
                RemapFileReferences(fileLink, newPath);
            }
        }

        void MoveDirectory(FileSystemLink directoryLink, CancellationToken innerToken) {
            if (destinationIsFile) {
                logger.Here().Warning("Cannot move directory {Path} to file {Destination}, skipping", directoryLink, destination);
                return;
            }

            // Reaching point of no return - after this alterations will be made 
            if (innerToken.IsCancellationRequested) return;

            // Add directory name to the destination path
            var newDestinationPath = destination.FileSystem.Path.Combine(destination.DataRelativePath.Path, directoryLink.Name);
            destination = new FileSystemLink(destination.DataSource, newDestinationPath); 

            var remaps = directoryLink
                .EnumerateFileLinks(true)
                .Select(fileLink => {
                    var relativePath =
                        directoryLink.FileSystem.Path.GetRelativePath(directoryLink.DataRelativePath.Path, fileLink.DataRelativePath.Path);
                    var destinationPath = destination.FileSystem.Path.Combine(destination.DataRelativePath.Path, relativePath);
                    var linkDestination = new FileSystemLink(destination.DataSource, destinationPath);
                    return (fileLink, linkDestination);
                })
                .ToArray();

            // Move the asset
            if (!FileSystemMove(directoryLink, destination, copy, innerToken)) return;

            if (doRemap) {
                foreach (var (fileLink, linkDestination) in remaps) {
                    RemapFileReferences(fileLink, linkDestination);
                }
            }
        }
    }

    public void RemapDirectoryReferences(FileSystemLink oldLink, FileSystemLink newLink) {
        foreach (var fileLink in oldLink.EnumerateFileLinks(true)) {
            var relativePath = oldLink.FileSystem.Path.GetRelativePath(oldLink.DataRelativePath.Path, fileLink.DataRelativePath.Path);
            var destinationPath = newLink.FileSystem.Path.Combine(newLink.DataRelativePath.Path, oldLink.Name, relativePath);
            var linkDestination = new FileSystemLink(newLink.DataSource, destinationPath);
            RemapFileReferences(fileLink, linkDestination);
        }
    }

    public void RemapFileReferences(FileSystemLink oldLink, FileSystemLink newLink) {
        // Remap references in records
        RemapRecordReferences(oldLink, newLink);

        // Remap references in NIFs
        RemapAssetReferences(oldLink, newLink);
    }

    private void RemapAssetReferences(FileSystemLink oldLink, FileSystemLink newLink) {
        var assetLink = assetTypeService.GetAssetLink(oldLink.DataRelativePath);
        if (assetLink is null) {
            logger.Here().Warning("Couldn't find asset type for {Path}", oldLink.DataRelativePath);
            return;
        }

        foreach (var reference in referenceService.GetAssetReferences(assetLink)) {
            if (dataSourceService.TryGetFileLink(reference, out var referenceFileLink)) {
                modelModificationService.RemapLinks(
                    referenceFileLink,
                    p => !p.IsNullOrWhitespace() && oldLink.DataRelativePath.Path.EndsWith(p, DataRelativePath.PathComparison),
                    newLink.DataRelativePath.Path);
            } else {
                logger.Here().Warning("Couldn't find file link {Reference} of {Path}", reference, oldLink.FullPath);
            }
        }
    }

    private void RemapRecordReferences(FileSystemLink oldLink, FileSystemLink newLink) {
        var assetLink = assetTypeService.GetAssetLink(oldLink.DataRelativePath);
        if (assetLink is null) {
            logger.Here().Warning("Couldn't find asset type for {Path}", oldLink.DataRelativePath);
            return;
        }

        // Path without the base folder prefix
        // i.e. Change meshes\clutter\test\test.nif to clutter\test.nif
        var shortenedDataRelativePath = oldLink.FileSystem.Path.GetRelativePath(assetLink.Type.BaseFolder, newLink.DataRelativePath.Path);

        foreach (var formLink in referenceService.GetRecordReferences(assetLink)) {
            if (!linkCacheProvider.LinkCache.TryResolve(formLink, out var record)) continue;

            var recordOverride = recordController.GetOrAddOverride(record);
            recordOverride.RemapListedAssetLinks(new Dictionary<IAssetLinkGetter, string>(AssetLinkEqualityComparer.Instance) {
                { assetLink, shortenedDataRelativePath },
            });
        }
    }

    private bool FileSystemMove(FileSystemLink origin, FileSystemLink destination, bool copy, CancellationToken token) {
        var destinationDirectory = destination.FileSystem.Path.GetDirectoryName(destination.FullPath);

        try {
            if (destinationDirectory is not null) destination.FileSystem.Directory.CreateDirectory(destinationDirectory);

            if (origin.IsFile) {
                if (token.IsCancellationRequested) return false;

                if (copy) {
                    origin.FileSystem.File.Copy(origin.FullPath, destination.FullPath);
                } else {
                    origin.FileSystem.File.Move(origin.FullPath, destination.FullPath);
                }
            } else {
                if (token.IsCancellationRequested) return false;

                if (copy) {
                    origin.FileSystem.Directory.DeepCopy(origin.FullPath, destination.FullPath);
                } else {
                    if (destination.Exists()) {
                        // If the destination directory already exists, integrate the contents with recursive FileSystemMove calls
                        foreach (var link in origin.EnumerateAllLinks(true)) {
                            var relativePath = origin.FileSystem.Path.GetRelativePath(origin.DataRelativePath.Path, link.DataRelativePath.Path);
                            var destinationPath = destination.FileSystem.Path.Combine(destination.DataRelativePath.Path, relativePath);
                            var linkDestination = new FileSystemLink(destination.DataSource, destinationPath);
                            FileSystemMove(link, linkDestination, copy, token);
                        }
                        
                        // If directory is empty after moving files, delete it
                        if (!origin.EnumerateAllLinks(true).Any()) {
                            origin.FileSystem.Directory.Delete(origin.FullPath, true);
                        }
                    } else {
                        origin.FileSystem.Directory.Move(origin.FullPath, destination.FullPath);
                    }
                }
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
