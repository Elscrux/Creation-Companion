using System.IO.Abstractions;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetController : IAssetController {
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    public AssetController(
        IFileSystem fileSystem,
        ILogger logger) {
        _fileSystem = fileSystem;
        _logger = logger;

    }

    public void Move(string path, string destination) {
        var destinationDirectory = _fileSystem.Path.GetDirectoryName(destination);

        try {
            if (destinationDirectory != null) _fileSystem.Directory.CreateDirectory(destinationDirectory);

            if (_fileSystem.File.Exists(path)) {
                _fileSystem.File.Move(path, destination);
            } else {
                _fileSystem.Directory.Move(path, destination);
            }
        } catch (Exception e) {
            _logger.Warning("Couldn't move {File} to {Dir}: {Exception}", path, destination, e.Message);
        }
    }

    public void Delete(string path) {
        if (!_fileSystem.Path.Exists(path)) return;

        try {
            var directoryInfo = _fileSystem.DirectoryInfo.New(path);
            if ((directoryInfo.Attributes & FileAttributes.Directory) == 0) {
                _fileSystem.File.Delete(path);
            } else {
                _fileSystem.Directory.Delete(path, true);
            }
        } catch (Exception e) {
            _logger.Warning("Couldn't delete {File}: {Exception}", path, e.Message);
        }
    }

    public void Redo() {
        throw new NotImplementedException();
    }

    public void Undo() {
        throw new NotImplementedException();
    }
}
