using System.Collections.Concurrent;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Noggog;
using Noggog.Utility;
namespace CreationEditor.Services.FileSystem.Validation;

public sealed class HashFileSystemValidation(
    IHashFileSystemValidationSerialization fileSystemValidationSerialization,
    IEnumerable<string> fileExtensions)
    : IFileSystemValidation {

    /// <summary>
    /// Depth of the directory tree to search for invalidated files in parallel.
    /// </summary>
    private const int MaxParallelLevel = 2;

    private readonly Lock _lock = new();
    private readonly ConcurrentDictionary<string, AsyncLock> _dataSourceLocks = [];

    public async Task<CacheValidationResult<DataRelativePath>> GetInvalidatedContent(FileSystemDataSource source) {
        AsyncLock asyncLock;
        lock (_lock) {
            asyncLock = _dataSourceLocks.GetOrAdd(source.Path, _ => new AsyncLock());
        }

        using var disposable = await asyncLock.WaitAsync();

        var validate = fileSystemValidationSerialization.Validate(source.Path);
        if (!validate || !fileSystemValidationSerialization.TryDeserialize(source.Path, out var fileSystemCacheData)) {
            var buildCache = await BuildCache(source);
            fileSystemValidationSerialization.Serialize(buildCache, source.Path);

            return CacheValidationResult<DataRelativePath>.FullyInvalid(() => {
                fileSystemValidationSerialization.Serialize(buildCache, source.Path);
            });
        }

        var invalidatedFiles = await ValidateDirectoryRec(fileSystemCacheData.RootDirectory, source.GetRootLink(), 1).ToArrayAsync();
        if (invalidatedFiles.Length == 0) return CacheValidationResult<DataRelativePath>.Valid();

        return CacheValidationResult<DataRelativePath>.PartlyInvalid(invalidatedFiles
                .Select(file => new DataRelativePath(source.FileSystem.Path.GetRelativePath(source.Path, file.FullPath)))
                .ToArray(),
            () => {
                UpdateCache(fileSystemCacheData, invalidatedFiles, source);
                fileSystemValidationSerialization.Serialize(fileSystemCacheData, source.Path);
            });

        async IAsyncEnumerable<DataSourceFileLink> ValidateDirectoryRec(
            HashDirectoryCacheData hashDirectoryCache,
            DataSourceDirectoryLink directoryLink,
            int level) {
            var nextLevel = level + 1;

            // Use DFS to search for new or changed files in all subdirectories 
            // For levels up until MaxParallelLevel, we can use run this recursive search in parallel
            if (level > MaxParallelLevel) {
                foreach (var subDirectoryName in directoryLink.EnumerateDirectoryLinks(false)) {
                    await foreach (var fileLink in DirectoryParse(subDirectoryName)) {
                        yield return fileLink;
                    }
                }
            } else {
                var directoryResults = await Task.WhenAll(
                    directoryLink.EnumerateDirectoryLinks(false)
                        .Select(subDirectoryName => Task.Run(() => DirectoryParse(subDirectoryName))));

                foreach (var files in directoryResults) {
                    await foreach (var fileLink in files) {
                        yield return fileLink;
                    }
                }
            }

            // Check all files in this directory and compare their hashes with the cache
            foreach (var fileExtension in fileExtensions) {
                foreach (var fileLink in directoryLink.EnumerateFileLinks('*' + fileExtension, false)) {
                    var fileCache = hashDirectoryCache.Files.FirstOrDefault(d => DataRelativePath.PathComparer.Equals(d.Name, fileLink.Name));

                    if (fileCache is null || !fileLink.FileSystem.IsFileHashValid(fileLink.FullPath, fileCache.Hash)) {
                        yield return fileLink;
                    }
                }
            }

            async IAsyncEnumerable<DataSourceFileLink> DirectoryParse(DataSourceDirectoryLink subDirectoryLink) {
                var subDirectoryCache = hashDirectoryCache.SubDirectories
                    .FirstOrDefault(d => DataRelativePath.PathComparer.Equals(d.Name, subDirectoryLink.Name));

                if (subDirectoryCache is null) {
                    // No cache exists for this subdirectory, return all new files in this directory as invalid
                    foreach (var fileExtension in fileExtensions) {
                        foreach (var filePath in subDirectoryLink.EnumerateFileLinks('*' + fileExtension, true)) {
                            yield return filePath;
                        }
                    }
                } else {
                    await foreach (var filePath in ValidateDirectoryRec(subDirectoryCache, subDirectoryLink, nextLevel)) {
                        yield return filePath;
                    }
                }
            }
        }
    }

    private static void UpdateCache(
        HashFileSystemCacheData fileSystemCacheData,
        IEnumerable<DataSourceFileLink> invalidatedFiles,
        FileSystemDataSource dataSource) {
        foreach (var fileLink in invalidatedFiles) {
            var relativePath = fileLink.FileSystem.Path.GetRelativePath(dataSource.Path, fileLink.FullPath);

            var currentDirectory = fileSystemCacheData.RootDirectory;
            var strings = relativePath.Split(fileLink.FileSystem.Path.DirectorySeparatorChar, fileLink.FileSystem.Path.AltDirectorySeparatorChar);
            for (var i = 0; i < strings.Length; i++) {
                var s = strings[i];
                if (i == strings.Length - 1) {
                    // File
                    var fileHash = fileLink.FileSystem.GetFileHash(fileLink.FullPath);
                    currentDirectory.Files.RemoveWhere(d => DataRelativePath.PathComparer.Equals(d.Name, s));
                    currentDirectory.Files.Add(new HashFileCacheData(s, fileHash));
                } else {
                    // Directory
                    var directory = currentDirectory.SubDirectories.FirstOrDefault(d => DataRelativePath.PathComparer.Equals(d.Name, s));
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

    private async Task<HashFileSystemCacheData> BuildCache(FileSystemDataSource source) {
        var rootDirectory = await BuildDirectoryCache(source.GetRootLink(), 1)
         ?? new HashDirectoryCacheData(source.Name, [], []);

        return new HashFileSystemCacheData(source.FileSystem.GetHashBytesLength(), rootDirectory);

        async Task<HashDirectoryCacheData?> BuildDirectoryCache(DataSourceDirectoryLink directoryLink, int level) {
            var nextLevel = level + 1;

            // Build directories
            IList<HashDirectoryCacheData> subDirectories;
            if (level > MaxParallelLevel) {
                var d = new List<HashDirectoryCacheData>();
                foreach (var subDirectoryLink in directoryLink.EnumerateDirectoryLinks(false)) {
                    var hashDirectoryCacheData = await BuildDirectoryCache(subDirectoryLink, nextLevel);
                    if (hashDirectoryCacheData is not null) {
                        d.Add(hashDirectoryCacheData);
                    }
                }
                subDirectories = d;
            } else {
                var hashDirectoryCacheData = await Task.WhenAll(directoryLink.EnumerateDirectoryLinks(false)
                    .Select(dir => Task.Run(() => BuildDirectoryCache(dir, nextLevel))));

                subDirectories = hashDirectoryCacheData.WhereNotNull().ToArray();
            }

            // Build files
            var files = new List<HashFileCacheData>();
            foreach (var fileExtension in fileExtensions) {
                foreach (var filePath in directoryLink.EnumerateFileLinks('*' + fileExtension, false)) {
                    var hash = filePath.FileSystem.GetFileHash(filePath.FullPath);

                    files.Add(new HashFileCacheData(filePath.Name, hash));
                }
            }

            // Finalize cache
            if (subDirectories.Count == 0 && files.Count == 0) return null;

            return new HashDirectoryCacheData(directoryLink.Name, subDirectories, files);
        }
    }
}
