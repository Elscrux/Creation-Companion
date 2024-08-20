using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using Mutagen.Bethesda.Assets;
using Noggog;
namespace CreationEditor.Services.FileSystem.Validation;

public sealed class HashFileSystemValidation(
    IFileSystem fileSystem,
    IHashFileSystemValidationSerialization fileSystemValidationSerialization,
    string searchPattern = "*.*")
    : IFileSystemValidation {

    private const int MaxParallelLevel = 2;

    public async Task<CacheValidationResult<DataRelativePath>> GetInvalidatedContent(string source) {
        var validate = fileSystemValidationSerialization.Validate(source);
        if (!validate || !fileSystemValidationSerialization.TryDeserialize(source, out var fileSystemCacheData)) {
            var buildCache = await BuildCache(source);
            fileSystemValidationSerialization.Serialize(buildCache, source);

            return CacheValidationResult<DataRelativePath>.FullyInvalid(() => {
                fileSystemValidationSerialization.Serialize(buildCache, source);
            });
        }

        var invalidatedFiles = await ValidateDirectoryRec(fileSystemCacheData.RootDirectory, source, 1).ToArrayAsync();
        if (invalidatedFiles.Length == 0) return CacheValidationResult<DataRelativePath>.Valid();

        return CacheValidationResult<DataRelativePath>.PartlyInvalid(invalidatedFiles
                .Select(x => new DataRelativePath(fileSystem.Path.GetRelativePath(source, x)))
                .ToArray(),
            () => {
                UpdateCache(fileSystemCacheData, invalidatedFiles, source);
                fileSystemValidationSerialization.Serialize(fileSystemCacheData, source);
            });

        async IAsyncEnumerable<string> ValidateDirectoryRec(HashDirectoryCacheData hashDirectoryCache, string directoryPath, int level) {
            var nextLevel = level + 1;

            // Use DFS to search for new or changed files in all subdirectories 
            // For levels up until MaxParallelLevel, we can use run this recursive search in parallel
            if (level > MaxParallelLevel) {
                foreach (var subDirectoryName in fileSystem.Directory.EnumerateDirectories(directoryPath)) {
                    await foreach (var filePath in DirectoryParse(subDirectoryName)) {
                        yield return filePath;
                    }
                }
            } else {
                var directoryResults = await Task.WhenAll(
                    fileSystem.Directory.EnumerateDirectories(directoryPath)
                        .Select(subDirectoryName => Task.Run(() => DirectoryParse(subDirectoryName))));

                foreach (var asyncEnumerable in directoryResults) {
                    await foreach (var s in asyncEnumerable) {
                        yield return s;
                    }
                }
            }

            // Check all files in this directory and compare their hashes with the cache
            foreach (var filePath in fileSystem.Directory.EnumerateFiles(directoryPath, searchPattern)) {
                var fileName = fileSystem.Path.GetFileName(filePath);
                var fileCache = hashDirectoryCache.Files.FirstOrDefault(d => AssetCompare.PathComparer.Equals(d.Name, fileName));

                if (fileCache is null) {
                    yield return filePath;
                } else {
                    if (!fileSystem.IsFileHashValid(filePath, fileCache.Hash)) {
                        yield return filePath;
                    }
                }
            }

            async IAsyncEnumerable<string> DirectoryParse(string subDirectoryPath) {
                var subDirectoryName = fileSystem.Path.GetFileName(subDirectoryPath);
                var subDirectoryCache = hashDirectoryCache.SubDirectories
                    .FirstOrDefault(d => AssetCompare.PathComparer.Equals(d.Name, subDirectoryName));

                if (subDirectoryCache is null) {
                    // No cache exists for this subdirectory, return all new files in this directory as invalid
                    foreach (var filePath in fileSystem.Directory.EnumerateFiles(subDirectoryPath, searchPattern, SearchOption.AllDirectories)) {
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
            var relativePath = fileSystem.Path.GetRelativePath(rootDirectoryPath, filePath);

            var currentDirectory = fileSystemCacheData.RootDirectory;
            var strings = relativePath.Split(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar);
            for (var i = 0; i < strings.Length; i++) {
                var s = strings[i];
                if (i == strings.Length - 1) {
                    // File
                    var fileHash = fileSystem.GetFileHash(filePath);
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
         ?? new HashDirectoryCacheData(fileSystem.Path.GetFileName(rootDirectoryPath), [], []);

        return new HashFileSystemCacheData(fileSystem.GetHashBytesLength(), rootDirectory);

        async Task<HashDirectoryCacheData?> BuildDirectoryCache(string directoryPath, int level) {
            var nextLevel = level + 1;

            // Build directories
            IList<HashDirectoryCacheData> subDirectories;
            if (level > MaxParallelLevel) {
                var d = new List<HashDirectoryCacheData>();
                foreach (var subDirectoryPath in fileSystem.Directory.EnumerateDirectories(directoryPath)) {
                    var hashDirectoryCacheData = await BuildDirectoryCache(subDirectoryPath, nextLevel);
                    if (hashDirectoryCacheData is not null) {
                        d.Add(hashDirectoryCacheData);
                    }
                }
                subDirectories = d;
            } else {
                var hashDirectoryCacheData = await Task.WhenAll(fileSystem.Directory.EnumerateDirectories(directoryPath)
                    .Select(dir => Task.Run(() => BuildDirectoryCache(dir, nextLevel))));

                subDirectories = hashDirectoryCacheData.NotNull().ToArray();
            }

            // Build files
            var files = new List<HashFileCacheData>();
            foreach (var filePath in fileSystem.Directory.EnumerateFiles(directoryPath, searchPattern)) {
                var hash = fileSystem.GetFileHash(filePath);

                var fileName = fileSystem.Path.GetFileName(filePath);
                files.Add(new HashFileCacheData(fileName, hash));
            }

            // Finalize cache
            if (subDirectories.Count == 0 && files.Count == 0) return null;

            var directoryName = fileSystem.Path.GetFileName(directoryPath);
            return new HashDirectoryCacheData(directoryName, subDirectories, files);
        }
    }
}
