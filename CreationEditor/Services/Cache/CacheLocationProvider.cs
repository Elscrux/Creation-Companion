using System.IO.Abstractions;
using Noggog;
namespace CreationEditor.Services.Cache;

public sealed class CacheLocationProvider : ICacheLocationProvider {
    private const string CacheDirectory = "MutagenCache";
    private const string CacheExtension = "cache";
    private const string TempCacheExtension = "temp";

    private readonly IFileSystem _fileSystem;
    private readonly string _subDirectory;

    public CacheLocationProvider(
        IFileSystem fileSystem,
        string subDirectory) {
        _fileSystem = fileSystem;
        _subDirectory = subDirectory;

    }

    private DirectoryPath CacheDirPath() => _fileSystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CacheDirectory, _subDirectory);
    public FilePath CacheFile(string name) => _fileSystem.Path.Combine(CacheDirPath(), $"{name}.{CacheExtension}");
    public FilePath TempCacheFile(string name) => _fileSystem.Path.Combine(CacheDirPath(), $"{name}.{TempCacheExtension}");
}
