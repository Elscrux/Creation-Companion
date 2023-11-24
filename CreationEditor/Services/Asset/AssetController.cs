using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetController : IAssetController {
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IDeleteDirectoryProvider _deleteDirectoryProvider;
    private readonly IAssetProvider _assetProvider;
    private readonly IModelModificationService _modelModificationService;
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IRecordController _recordController;
    private readonly ILogger _logger;

    public AssetController(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IDeleteDirectoryProvider deleteDirectoryProvider,
        IAssetProvider assetProvider,
        IModelModificationService modelModificationService,
        ILinkCacheProvider linkCacheProvider,
        IRecordController recordController,
        ILogger logger) {
        _fileSystem = fileSystem;
        _assetProvider = assetProvider;
        _linkCacheProvider = linkCacheProvider;
        _dataDirectoryProvider = dataDirectoryProvider;
        _deleteDirectoryProvider = deleteDirectoryProvider;
        _modelModificationService = modelModificationService;
        _recordController = recordController;
        _logger = logger;
    }

    private string CreateDeletePath(string path) {
        var relativePath = _fileSystem.Path.GetRelativePath(_dataDirectoryProvider.Path, path);

        return _fileSystem.Path.Combine(_deleteDirectoryProvider.DeleteDirectory, relativePath);
    }

    public void Delete(string path, CancellationToken token = default) {
        try {
            MoveInternal(path, CreateDeletePath(path), true, token);
        } catch (Exception e) {
            _logger.Here().Error("Couldn't delete {Path}: {Exception}", path, e.Message);
        }
    }

    public void Move(string path, string destination, CancellationToken token = default) {
        try {
            MoveInternal(path, destination, false, token);
        } catch (Exception e) {
            _logger.Here().Error("Couldn't move {Path} to {Destination}: {Exception}", path, destination, e.Message);
        }
    }

    public void Rename(string path, string newName, CancellationToken token = default) {
        var directoryPath = _fileSystem.Path.GetDirectoryName(path);
        if (directoryPath != null) {
            try {
                MoveInternal(path, _fileSystem.Path.Combine(directoryPath, newName), true, token);
            } catch (Exception e) {
                _logger.Here().Error("Couldn't rename {Path} to {NewName}: {Exception}", path, newName, e.Message);
            }
        } else {
            _logger.Here().Warning("Couldn't find path to base directory of {Path}", path);
        }
    }

    private void MoveInternal(string path, string destination, bool rename, CancellationToken token) {
        var isFile = _fileSystem.Path.HasExtension(path);

        // Get the parent directory for a file path or self for a directory path
        var baseDirectory = isFile ? _fileSystem.Path.GetDirectoryName(path) : path;
        if (baseDirectory is null) return;

        var assetContainer = _assetProvider.GetAssetContainer(baseDirectory, token);

        var basePath = assetContainer.Path;
        var baseIsFile = isFile || _fileSystem.Path.HasExtension(basePath);
        var destinationIsFile = _fileSystem.Path.HasExtension(destination);

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
            var fullNewPath = destinationIsFile
                // If the destination is a file, we already have the full path
                ? destination
                // Otherwise, combine the destination directory with the rest of the path to the file
                : _fileSystem.Path.Combine(
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
                        : _fileSystem.Path.Combine(destination, _fileSystem.Path.GetFileName(basePath)),
                    string.Equals(basePath, assetFile.Path, AssetCompare.PathComparison)
                        ? _fileSystem.Path.GetFileName(basePath)
                        : _fileSystem.Path.GetRelativePath(basePath, assetFile.Path));

            var dataRelativePath = _fileSystem.Path.GetRelativePath(_dataDirectoryProvider.Path, fullNewPath);

            // Path without the base folder prefix
            // Change meshes\clutter\test\test.nif to clutter\test.nif
            var shortenedPath = _fileSystem.Path.GetRelativePath(assetFile.ReferencedAsset.AssetLink.Type.BaseFolder, dataRelativePath);

            // Reaching point of no return - after this alterations will be made 
            if (innerToken.IsCancellationRequested) return;

            // Move the asset
            if (!FileSystemMove(assetFile.Path, fullNewPath, innerToken)) return;

            // Remap references in records
            foreach (var formLink in assetFile.ReferencedAsset.RecordReferences) {
                if (!_linkCacheProvider.LinkCache.TryResolve(formLink, out var record)) continue;

                var recordOverride = _recordController.GetOrAddOverride(record);
                recordOverride.RemapListedAssetLinks(new Dictionary<IAssetLinkGetter, string>(AssetLinkEqualityComparer.Instance) { { assetFile.ReferencedAsset.AssetLink, shortenedPath } });
            }

            // Remap references in NIFs
            foreach (var reference in assetFile.ReferencedAsset.NifReferences) {
                var fullPath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, reference);
                _modelModificationService.RemapLinks(fullPath, p => !p.IsNullOrWhitespace() && assetFile.Path.EndsWith(p, AssetCompare.PathComparison), dataRelativePath);
            }
        }
    }

    private bool FileSystemMove(string path, string destination, CancellationToken token) {
        var destinationDirectory = _fileSystem.Path.GetDirectoryName(destination);

        try {
            if (destinationDirectory is not null) _fileSystem.Directory.CreateDirectory(destinationDirectory);

            if (_fileSystem.File.Exists(path)) {
                if (token.IsCancellationRequested) return false;

                _fileSystem.File.Move(path, destination);
            } else {
                if (token.IsCancellationRequested) return false;

                _fileSystem.Directory.Move(path, destination);
            }
        } catch (Exception e) {
            _logger.Here().Warning("Couldn't move {File} to {Dir}: {Exception}", path, destination, e.Message);
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
