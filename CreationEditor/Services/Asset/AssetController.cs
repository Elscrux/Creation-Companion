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

    public void Open(IDataSourceLink link) {
        if (!link.Exists()) return;

        // Open the file via the standard program
        Process.Start(new ProcessStartInfo {
            FileName = link.FullPath,
            WorkingDirectory = link.ParentDirectory?.FullPath,
            UseShellExecute = true,
            Verb = "open",
        });
    }

    public void Delete(IDataSourceLink link, CancellationToken token = default) {
        try {
            var deletePath = link.FileSystem.Path.Combine(deleteDirectoryProvider.DeleteDirectory, link.DataRelativePath.Path);

            switch (link) {
                case DataSourceDirectoryLink directoryLink:
                    var deleteDirectoryLink = new DataSourceDirectoryLink(link.DataSource, deletePath);
                    FileSystemMove(directoryLink, deleteDirectoryLink, token);
                    break;
                case DataSourceFileLink fileLink:
                    var deleteFileLink = new DataSourceFileLink(link.DataSource, deletePath);
                    FileSystemMove(fileLink, deleteFileLink, token);
                    break;
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't delete {Path}: {Exception}", link, e.Message);
        }
    }

    public void Copy(IDataSourceLink origin, IDataSourceLink destination, CancellationToken token = default) {
        try {
            switch (origin) {
                case DataSourceDirectoryLink directoryLink: {
                    var dest = destination switch {
                        DataSourceDirectoryLink destinationDirectoryLink => destinationDirectoryLink,
                        _ => throw new InvalidOperationException("Cannot copy directory to file"),
                    };

                    var links = GetRemapLinks(directoryLink, dest);
                    if (FileSystemCopy(directoryLink, dest, token)) {
                        RemapLinks(links);
                    }
                    break;
                }
                case DataSourceFileLink fileLink: {
                    var dest = destination switch {
                        DataSourceFileLink destinationFileLink => destinationFileLink,
                        DataSourceDirectoryLink destinationDirectoryLink =>
                            new DataSourceFileLink(
                                destinationDirectoryLink.DataSource,
                                destinationDirectoryLink.FileSystem.Path.Combine(destinationDirectoryLink.DataRelativePath.Path, fileLink.Name)),
                    };

                    var links = GetRemapLinks(fileLink, dest);
                    if (FileSystemCopy(fileLink, dest, token)) {
                        RemapLinks(links);
                    }
                    break;
                }
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't move {Path} to {Destination}: {Exception}", origin, destination, e.Message);
        }
    }

    public void Move(IDataSourceLink origin, IDataSourceLink destination, CancellationToken token = default) {
        try {
            switch (origin) {
                case DataSourceDirectoryLink directoryLink: {
                    var dest = destination switch {
                        DataSourceDirectoryLink destinationDirectoryLink => destinationDirectoryLink,
                        _ => throw new InvalidOperationException("Cannot move directory to file"),
                    };

                    var links = GetRemapLinks(directoryLink, dest);
                    if (FileSystemMove(directoryLink, dest, token)) {
                        RemapLinks(links);
                    }
                    break;
                }
                case DataSourceFileLink fileLink: {
                    var dest = destination switch {
                        DataSourceFileLink destinationFileLink => destinationFileLink,
                        DataSourceDirectoryLink destinationDirectoryLink =>
                            new DataSourceFileLink(
                                destinationDirectoryLink.DataSource,
                                destinationDirectoryLink.FileSystem.Path.Combine(destinationDirectoryLink.DataRelativePath.Path, fileLink.Name)),
                    };

                    var links = GetRemapLinks(fileLink, dest);
                    if (FileSystemMove(fileLink, dest, token)) {
                        RemapLinks(links);
                    }
                    break;
                }
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Couldn't move {Path} to {Destination}: {Exception}", origin, destination, e.Message);
        }
    }

    public void Rename(IDataSourceLink origin, string newName, CancellationToken token = default) {
        var directoryPath = origin.ParentDirectory?.FullPath;

        if (directoryPath is not null) {
            try {
                var newPath = origin.FileSystem.Path.Combine(directoryPath, origin.Name);
                switch (origin) {
                    case DataSourceFileLink fileLink: {
                        var destinationFileLink = new DataSourceFileLink(fileLink.DataSource, newPath);
                        var links = GetRemapLinks(fileLink, destinationFileLink);
                        if (FileSystemMove(fileLink, destinationFileLink, token)) {
                            RemapLinks(links);
                        }
                        break;
                    }
                    case DataSourceDirectoryLink directoryLink: {
                        var destinationDirectoryLink = new DataSourceDirectoryLink(directoryLink.DataSource, newPath);
                        var links = GetRemapLinks(directoryLink, destinationDirectoryLink);
                        if (FileSystemMove(directoryLink, destinationDirectoryLink, token)) {
                            RemapLinks(links);
                        }
                        break;
                    }
                }
            } catch (Exception e) {
                logger.Here().Error(e, "Couldn't rename {Path} to {NewName}: {Exception}", origin, newName, e.Message);
            }
        } else {
            logger.Here().Warning("Couldn't find path to base directory of {Path}", origin);
        }
    }

    public void RemapDirectoryReferences(DataSourceDirectoryLink oldDirectoryLink, DataSourceDirectoryLink newDirectoryLink) {
        foreach (var fileLink in oldDirectoryLink.EnumerateFileLinks(true)) {
            var relativePath = oldDirectoryLink.FileSystem.Path.GetRelativePath(oldDirectoryLink.DataRelativePath.Path, fileLink.DataRelativePath.Path);
            var destinationPath = newDirectoryLink.FileSystem.Path.Combine(newDirectoryLink.DataRelativePath.Path, oldDirectoryLink.Name, relativePath);
            var linkDestination = new DataSourceFileLink(newDirectoryLink.DataSource, destinationPath);
            RemapFileReferences(fileLink, linkDestination);
        }
    }

    public void RemapFileReferences(DataSourceFileLink oldFileLink, DataSourceFileLink newFileLink) {
        // Remap references in records
        RemapRecordReferences(oldFileLink, newFileLink);

        // Remap references in NIFs
        RemapAssetReferences(oldFileLink, newFileLink);
    }

    private void RemapAssetReferences(DataSourceFileLink oldFileLink, DataSourceFileLink newFileLink) {
        var assetLink = assetTypeService.GetAssetLink(oldFileLink.DataRelativePath);
        if (assetLink is null) {
            logger.Here().Warning("Couldn't find asset type for {Path}", oldFileLink.DataRelativePath);
            return;
        }

        foreach (var reference in referenceService.GetAssetReferences(assetLink)) {
            if (dataSourceService.TryGetFileLink(reference, out var referenceFileLink)) {
                modelModificationService.RemapLinks(
                    referenceFileLink,
                    p => !p.IsNullOrWhitespace() && oldFileLink.DataRelativePath.Path.EndsWith(p, DataRelativePath.PathComparison),
                    newFileLink.DataRelativePath.Path);
            } else {
                logger.Here().Warning("Couldn't find file link {Reference} of {Path}", reference, oldFileLink.FullPath);
            }
        }
    }

    private void RemapRecordReferences(DataSourceFileLink oldFileLink, DataSourceFileLink newFileLink) {
        var assetLink = assetTypeService.GetAssetLink(oldFileLink.DataRelativePath);
        if (assetLink is null) {
            logger.Here().Warning("Couldn't find asset type for {Path}", oldFileLink.DataRelativePath);
            return;
        }

        // Path without the base folder prefix
        // i.e. Change meshes\clutter\test\test.nif to clutter\test.nif
        var shortenedDataRelativePath = oldFileLink.FileSystem.Path.GetRelativePath(assetLink.Type.BaseFolder, newFileLink.DataRelativePath.Path);

        foreach (var formLink in referenceService.GetRecordReferences(assetLink)) {
            if (!linkCacheProvider.LinkCache.TryResolve(formLink, out var record)) continue;

            var recordOverride = recordController.GetOrAddOverride(record);
            recordOverride.RemapListedAssetLinks(new Dictionary<IAssetLinkGetter, string>(AssetLinkEqualityComparer.Instance) {
                { assetLink, shortenedDataRelativePath },
            });
        }
    }

    private bool FileSystemCopy(DataSourceFileLink origin, DataSourceFileLink destination, CancellationToken token) {
        var destinationDirectory = destination.FileSystem.Path.GetDirectoryName(destination.FullPath);

        try {
            if (destinationDirectory is not null) destination.FileSystem.Directory.CreateDirectory(destinationDirectory);

            if (token.IsCancellationRequested) return false;

            origin.FileSystem.File.Copy(origin.FullPath, destination.FullPath);
        } catch (Exception e) {
            logger.Here().Warning(e, "Couldn't copy {Origin} to {Destination}: {Exception}", origin, destination, e.Message);
            return false;
        }

        return true;
    }

    private bool FileSystemCopy(DataSourceDirectoryLink origin, DataSourceDirectoryLink destination, CancellationToken token) {
        var destinationDirectory = destination.FileSystem.Path.GetDirectoryName(destination.FullPath);

        try {
            if (destinationDirectory is not null) destination.FileSystem.Directory.CreateDirectory(destinationDirectory);

            if (token.IsCancellationRequested) return false;

            origin.FileSystem.Directory.DeepCopy(origin.FullPath, destination.FullPath);
        } catch (Exception e) {
            logger.Here().Warning(e, "Couldn't copy {Origin} to {Destination}: {Exception}", origin, destination, e.Message);
            return false;
        }

        return true;
    }

    private bool FileSystemMove(DataSourceFileLink origin, DataSourceFileLink destination, CancellationToken token) {
        var destinationDirectory = destination.FileSystem.Path.GetDirectoryName(destination.FullPath);

        try {
            if (destinationDirectory is not null) destination.FileSystem.Directory.CreateDirectory(destinationDirectory);

            if (token.IsCancellationRequested) return false;

            origin.FileSystem.File.Move(origin.FullPath, destination.FullPath);
        } catch (Exception e) {
            logger.Here().Warning(e, "Couldn't move {Origin} to {Destination}: {Exception}", origin, destination, e.Message);
            return false;
        }

        return true;
    }

    private bool FileSystemMove(DataSourceDirectoryLink origin, DataSourceDirectoryLink destination, CancellationToken token) {
        var destinationDirectory = destination.FileSystem.Path.GetDirectoryName(destination.FullPath);

        try {
            if (destinationDirectory is not null) destination.FileSystem.Directory.CreateDirectory(destinationDirectory);

            if (token.IsCancellationRequested) return false;

            if (destination.Exists()) {
                // If the destination directory already exists, integrate the contents with recursive FileSystemMove calls
                foreach (var link in origin.EnumerateAllLinks(true)) {
                    var relativePath = origin.FileSystem.Path.GetRelativePath(origin.DataRelativePath.Path, link.DataRelativePath.Path);
                    var destinationPath = destination.FileSystem.Path.Combine(destination.DataRelativePath.Path, relativePath);
                    switch (link) {
                        case DataSourceFileLink fileLink:
                            var destinationFileLink = new DataSourceFileLink(destination.DataSource, destinationPath);
                            FileSystemMove(fileLink, destinationFileLink, token);
                            break;
                        case DataSourceDirectoryLink directoryLink:
                            var destinationDirectoryLink = new DataSourceDirectoryLink(destination.DataSource, destinationPath);
                            FileSystemMove(directoryLink, destinationDirectoryLink, token);
                            break;
                    }
                }

                // If directory is empty after moving files, delete it
                if (!origin.EnumerateAllLinks(true).Any()) {
                    origin.FileSystem.Directory.Delete(origin.FullPath, true);
                }
            } else {
                origin.FileSystem.Directory.Move(origin.FullPath, destination.FullPath);
            }
        } catch (Exception e) {
            logger.Here().Warning(e, "Couldn't move {Origin} to {Destination}: {Exception}", origin, destination, e.Message);
            return false;
        }

        return true;
    }

    private static IReadOnlyList<IUpdate<DataSourceFileLink>> GetRemapLinks(DataSourceFileLink origin, DataSourceFileLink destination) {
        return [new Update<DataSourceFileLink>(origin, destination)];
    }

    private static IReadOnlyList<IUpdate<DataSourceFileLink>> GetRemapLinks(DataSourceDirectoryLink origin, DataSourceDirectoryLink destination) {
        return origin
            .EnumerateFileLinks(true)
            .Select(fileLink => {
                var relativePath = origin.FileSystem.Path.GetRelativePath(origin.DataRelativePath.Path, fileLink.DataRelativePath.Path);
                var destinationPath = destination.FileSystem.Path.Combine(destination.DataRelativePath.Path, relativePath);
                var linkDestination = new DataSourceFileLink(destination.DataSource, destinationPath);
                return new Update<DataSourceFileLink>(fileLink, linkDestination);
            })
            .ToArray();
    }

    private void RemapLinks(IReadOnlyList<IUpdate<DataSourceFileLink>> links) {
        foreach (var update in links) {
            RemapFileReferences(update.Old, update.New);
        }
    }

    public void Redo() {
        throw new NotImplementedException();
    }

    public void Undo() {
        throw new NotImplementedException();
    }
}
