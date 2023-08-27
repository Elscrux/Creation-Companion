using System.IO.Abstractions;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using Serilog;
namespace CreationEditor.Services.FileSystem.Validation;

public sealed class BinaryFileSystemValidationSerialization : IHashFileSystemValidationSerialization {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;
    private readonly ICacheLocationProvider _cacheLocationProvider;
    private readonly Version _version = new(1, 0);

    public BinaryFileSystemValidationSerialization(
        ILogger logger,
        Func<string[], ICacheLocationProvider> cacheLocationProviderFactory,
        IFileSystem fileSystem) {
        _logger = logger;
        _fileSystem = fileSystem;
        _cacheLocationProvider = cacheLocationProviderFactory(new[] { "Validation", "FileSystem" });
    }

    public bool Validate(string rootDirectoryPath) {
        var cacheFile = _cacheLocationProvider.CacheFile(rootDirectoryPath);

        // Check if cache exist
        if (!_fileSystem.File.Exists(cacheFile)) return false;

        try {
            using var fileSystemStream = _fileSystem.File.OpenRead(cacheFile);
            using var reader = new BinaryReader(fileSystemStream);

            // Read serialization version
            var versionString = reader.ReadString();
            if (!Version.TryParse(versionString, out var version)) return false;
            if (!_version.Equals(version)) return false;
        } catch (Exception e) {

            _logger.Here().Warning("Failed to validate cache file {File}: {Exception}", cacheFile, e.Message);
            return false;
        }

        return true;
    }

    public HashFileSystemCacheData Deserialize(string rootDirectoryPath) {
        var cacheFile = _cacheLocationProvider.CacheFile(rootDirectoryPath);

        using var fileSystemStream = _fileSystem.File.OpenRead(cacheFile);
        return Deserialize(new BinaryReader(fileSystemStream));
    }

    public HashFileSystemCacheData Deserialize(BinaryReader reader) {
        // Skip serialization version
        reader.ReadString();

        // Read cache data
        var hashLength = reader.ReadInt32();
        var hashDirectoryCacheData = Directory();

        return new HashFileSystemCacheData(hashLength, hashDirectoryCacheData);

        HashDirectoryCacheData Directory() {
            var name = reader.ReadString();

            var subDirectoryCount = reader.ReadInt32();
            var subDirectories = new List<HashDirectoryCacheData>();
            for (var i = 0; i < subDirectoryCount; i++) {
                subDirectories.Add(Directory());
            }

            var fileCount = reader.ReadInt32();
            var files = new List<HashFileCacheData>();
            for (var i = 0; i < fileCount; i++) {
                files.Add(File());
            }

            return new HashDirectoryCacheData(name, subDirectories, files);
        }

        HashFileCacheData File() {
            var name = reader.ReadString();
            var hash = reader.ReadBytes(hashLength);

            return new HashFileCacheData(name, hash);
        }
    }

    public void Serialize(HashFileSystemCacheData fileSystemCacheData, string rootDirectoryPath) {
        var cacheFile = _cacheLocationProvider.CacheFile(rootDirectoryPath);
        var info = _fileSystem.FileInfo.New(cacheFile);
        info.Directory?.Create();

        using var fileSystemStream = _fileSystem.File.OpenWrite(cacheFile);
        Serialize(fileSystemCacheData, new BinaryWriter(fileSystemStream));
    }

    public void Serialize(HashFileSystemCacheData fileSystemCacheData, BinaryWriter writer) {
        writer.Write(_version.ToString());
        writer.Write(fileSystemCacheData.HashLength);
        Directory(fileSystemCacheData.RootDirectory);
        return;

        void Directory(HashDirectoryCacheData hashDirectoryCacheData) {
            writer.Write(hashDirectoryCacheData.Name);

            writer.Write(hashDirectoryCacheData.SubDirectories.Count);
            foreach (var subDirectory in hashDirectoryCacheData.SubDirectories) {
                Directory(subDirectory);
            }

            writer.Write(hashDirectoryCacheData.Files.Count);
            foreach (var file in hashDirectoryCacheData.Files) {
                File(file);
            }
        }

        void File(HashFileCacheData hashFileCacheData) {
            writer.Write(hashFileCacheData.Name);
            writer.Write(hashFileCacheData.Hash);
        }
    }
}
