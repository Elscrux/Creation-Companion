using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using Noggog;
namespace CreationEditor.Services.FileSystem.Validation;

public sealed class HashFileSystemValidation : IFileSystemValidation {
    private const int MaxParallelLevel = 2;

    private readonly IFileSystem _fileSystem;
    private readonly string _searchPattern;
    private readonly IHashFileSystemValidationSerialization _fileSystemValidationSerialization;

    public HashFileSystemValidation(
        IFileSystem fileSystem,
        IHashFileSystemValidationSerialization fileSystemValidationSerialization,
        string searchPattern = "*.*") {
        _fileSystem = fileSystem;
        _fileSystemValidationSerialization = fileSystemValidationSerialization;
        _searchPattern = searchPattern;
    }

    public async Task<CacheValidationResult<string>> GetInvalidatedContent(string source) {
        var validate = _fileSystemValidationSerialization.Validate(source);
        if (!validate || !_fileSystemValidationSerialization.TryDeserialize(source, out var fileSystemCacheData)) { 
            var buildCache = await BuildCache(source);
            _fileSystemValidationSerialization.Serialize(buildCache, source);

            return CacheValidationResult<string>.FullyInvalid();
        }

        var invalidatedFiles = await ValidateDirectoryRec(fileSystemCacheData.RootDirectory, source, 1).ToArrayAsync();
        if (invalidatedFiles.Length == 0) return CacheValidationResult<string>.Valid();

        // Do cache update process in the background
        Task.Run(() => {
            UpdateCache(fileSystemCacheData, invalidatedFiles, source);
            _fileSystemValidationSerialization.Serialize(fileSystemCacheData, source);
        });

        return CacheValidationResult<string>.PartlyInvalid(invalidatedFiles
            .Select(x => _fileSystem.Path.GetRelativePath(source, x))
            .ToArray());

        async IAsyncEnumerable<string> ValidateDirectoryRec(HashDirectoryCacheData hashDirectoryCache, string directoryPath, int level) {
            var nextLevel = level + 1;

            // Use DFS to search for new or changed files in all subdirectories 
            // For levels up until MaxParallelLevel, we can use run this recursive search in parallel
            if (level > MaxParallelLevel) {
                foreach (var subDirectoryName in _fileSystem.Directory.EnumerateDirectories(directoryPath)) {
                    await foreach (var filePath in DirectoryParse(subDirectoryName)) {
                        yield return filePath;
                    }
                }
            } else {
                var directoryResults = await Task.WhenAll(
                    _fileSystem.Directory.EnumerateDirectories(directoryPath)
                        .Select(subDirectoryName => Task.Run(() => DirectoryParse(subDirectoryName))));

                foreach (var asyncEnumerable in directoryResults) {
                    await foreach (var s in asyncEnumerable) {
                        yield return s;
                    }
                }
            }

            // Check all files in this directory and compare their hashes with the cache
            foreach (var filePath in _fileSystem.Directory.EnumerateFiles(directoryPath, _searchPattern)) {
                var fileName = _fileSystem.Path.GetFileName(filePath);
                var fileCache = hashDirectoryCache.Files.FirstOrDefault(d => AssetCompare.PathComparer.Equals(d.Name, fileName));

                if (fileCache is null) {
                    yield return filePath;
                } else {
                    if (!_fileSystem.IsFileHashValid(filePath, fileCache.Hash)) {
                        yield return filePath;
                    }
                }
            }
            yield break;

            async IAsyncEnumerable<string> DirectoryParse(string subDirectoryPath) {
                var subDirectoryName = _fileSystem.Path.GetFileName(subDirectoryPath);
                var subDirectoryCache = hashDirectoryCache.SubDirectories.FirstOrDefault(d => AssetCompare.PathComparer.Equals(d.Name, subDirectoryName));

                if (subDirectoryCache is null) {
                    // No cache exists for this subdirectory, return all new files in this directory as invalid
                    foreach (var filePath in _fileSystem.Directory.EnumerateFiles(subDirectoryPath, _searchPattern, SearchOption.AllDirectories)) {
                        yield return filePath;
                    }
                } else {
                    await foreach (var filePath in ValidateDirectoryRec(subDirectoryCache, subDirectoryPath, nextLevel)) {
                        yield return filePath;
                    }
                }
            }
        }
    }

    private void UpdateCache(HashFileSystemCacheData fileSystemCacheData, IEnumerable<string> invalidatedFiles, string rootDirectoryPath) {
        foreach (var filePath in invalidatedFiles) {
            var relativePath = _fileSystem.Path.GetRelativePath(rootDirectoryPath, filePath);

            var currentDirectory = fileSystemCacheData.RootDirectory;
            var strings = relativePath.Split(_fileSystem.Path.DirectorySeparatorChar, _fileSystem.Path.AltDirectorySeparatorChar);
            for (var i = 0; i < strings.Length; i++) {
                var s = strings[i];
                if (i == strings.Length - 1) {
                    // File
                    var fileHash = _fileSystem.GetFileHash(filePath);
                    currentDirectory.Files.RemoveWhere(d => AssetCompare.PathComparer.Equals(d.Name, s));
                    currentDirectory.Files.Add(new HashFileCacheData(s, fileHash));
                } else {
                    // Directory
                    var directory = currentDirectory.SubDirectories.FirstOrDefault(d => AssetCompare.PathComparer.Equals(d.Name, s));
                    if (directory is null) {
                        var subDirectory = new HashDirectoryCacheData(s, new List<HashDirectoryCacheData>(), new List<HashFileCacheData>());
                        currentDirectory.SubDirectories.Add(subDirectory);
                        currentDirectory = subDirectory;
                    } else {
                        currentDirectory = directory;
                    }
                }
            }
        }
    }

    private async Task<HashFileSystemCacheData> BuildCache(string rootDirectoryPath) {
        var rootDirectory = await BuildDirectoryCache(rootDirectoryPath, 1)
         ?? new HashDirectoryCacheData(
                _fileSystem.Path.GetFileName(rootDirectoryPath),
                Array.Empty<HashDirectoryCacheData>(),
                Array.Empty<HashFileCacheData>());

        return new HashFileSystemCacheData(_fileSystem.GetHashBytesLength(), rootDirectory);

        async Task<HashDirectoryCacheData?> BuildDirectoryCache(string directoryPath, int level) {
            var nextLevel = level + 1;

            // Build directories
            IList<HashDirectoryCacheData> subDirectories;
            if (level > MaxParallelLevel) {
                var d = new List<HashDirectoryCacheData>();
                foreach (var subDirectoryPath in _fileSystem.Directory.EnumerateDirectories(directoryPath)) {
                    var hashDirectoryCacheData = await BuildDirectoryCache(subDirectoryPath, nextLevel);
                    if (hashDirectoryCacheData is not null) {
                        d.Add(hashDirectoryCacheData);
                    }
                }
                subDirectories = d;
            } else {
                var hashDirectoryCacheData = await Task.WhenAll(_fileSystem.Directory.EnumerateDirectories(directoryPath)
                    .Select(dir => Task.Run(() => BuildDirectoryCache(dir, nextLevel))));

                subDirectories = hashDirectoryCacheData.NotNull().ToArray();
            }

            // Build files
            var files = new List<HashFileCacheData>();
            foreach (var filePath in _fileSystem.Directory.EnumerateFiles(directoryPath, _searchPattern)) {
                var hash = _fileSystem.GetFileHash(filePath);

                var fileName = _fileSystem.Path.GetFileName(filePath);
                files.Add(new HashFileCacheData(fileName, hash));
            }

            // Finalize cache
            if (subDirectories.Count == 0 && files.Count == 0) return null;

            var directoryName = _fileSystem.Path.GetFileName(directoryPath);
            return new HashDirectoryCacheData(directoryName, subDirectories, files);
        }
    }
}
