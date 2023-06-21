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
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IRecordController _recordController;
    private readonly ILogger _logger;

    public AssetController(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IDeleteDirectoryProvider deleteDirectoryProvider,
        IAssetProvider assetProvider,
        IModelModificationService modelModificationService,
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        ILogger logger) {
        _fileSystem = fileSystem;
        _assetProvider = assetProvider;
        _editorEnvironment = editorEnvironment;
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

    public void Delete(string path) => MoveInternal(path, CreateDeletePath(path), true);

    public void Move(string path, string destination) => MoveInternal(path, destination, false);

    public void Rename(string path, string destination) => MoveInternal(path, destination, true);

    private void MoveInternal(string path, string destination, bool rename) {
        var assetContainer = _assetProvider.GetAssetContainer(path);

        var basePath = assetContainer.Path;
        var baseIsFile = _fileSystem.Path.HasExtension(basePath);
        var destinationIsFile = _fileSystem.Path.HasExtension(destination);

        foreach (var file in assetContainer.GetAllChildren(a => a.Children, true)) {
            if (file is not AssetFile assetFile) continue;

            // Calculate the full path of that the file should be moved to
            var fullNewPath = destinationIsFile
                // If the destination is a file, we already have the full path
                ? destination
                // Otherwise, combine the destination directory with the rest of the path to the file
                : _fileSystem.Path.Combine(
                    rename || baseIsFile
                        // Example Renaming meshes\clutter to meshes\clutter-new
                        // basePath:          meshes\clutter
                        // childOrSelf.Path:  meshes\clutter\test.nif
                        // destination:       meshes\clutter-new\
                        // fullNewPath:       meshes\clutter-new\test.nif
                        ? destination
                        // Example Moving meshes\clutter to meshes\clutter-new
                        // basePath:          meshes\clutter
                        // childOrSelf.Path:  meshes\clutter\test.nif
                        // destination:       meshes\clutter-new\
                        // fullNewPath:       meshes\clutter-new\clutter\test.nif
                        : _fileSystem.Path.Combine(destination, _fileSystem.Path.GetFileName(basePath)),
                    string.Equals(basePath, file.Path, AssetCompare.PathComparison)
                        ? _fileSystem.Path.GetFileName(basePath)
                        : _fileSystem.Path.GetRelativePath(basePath, file.Path));

            var dataRelativePath = _fileSystem.Path.GetRelativePath(_dataDirectoryProvider.Path, fullNewPath);

            // Path without the base folder prefix
            // Change meshes\clutter\test\test.nif to clutter\test.nif
            var shortenedPath = _fileSystem.Path.GetRelativePath(assetFile.ReferencedAsset.AssetLink.Type.BaseFolder, dataRelativePath);

            // Move the asset
            if (!FileSystemMove(file.Path, fullNewPath)) continue;

            // Remap references in records
            foreach (var formLink in assetFile.ReferencedAsset.RecordReferences) {
                if (!_editorEnvironment.LinkCache.TryResolve(formLink, out var record)) continue;

                var recordOverride = _recordController.GetOrAddOverride(record);
                recordOverride.RemapListedAssetLinks(new Dictionary<IAssetLinkGetter, string>(AssetLinkEqualityComparer.Instance) { { assetFile.ReferencedAsset.AssetLink, shortenedPath } });
            }

            // Remap references in NIFs
            foreach (var reference in assetFile.ReferencedAsset.NifReferences) {
                var fullPath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, reference);
                _modelModificationService.RemapReferences(fullPath, path => !path.IsNullOrWhitespace() && file.Path.EndsWith(path, AssetCompare.PathComparison), dataRelativePath);
            }
        }
    }

    private bool FileSystemMove(string path, string destination) {
        var destinationDirectory = _fileSystem.Path.GetDirectoryName(destination);

        try {
            if (destinationDirectory is not null) _fileSystem.Directory.CreateDirectory(destinationDirectory);

            if (_fileSystem.File.Exists(path)) {
                _fileSystem.File.Move(path, destination);
            } else {
                _fileSystem.Directory.Move(path, destination);
            }
        } catch (Exception e) {
            _logger.Warning("Couldn't move {File} to {Dir}: {Exception}", path, destination, e.Message);
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
