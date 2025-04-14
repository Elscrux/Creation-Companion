using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using Mutagen.Bethesda.Assets;
using Noggog;
namespace CreationEditor.Services.FileSystem.Validation;

public sealed class HashFileSystemValidation(
    IHashFileSystemValidationSerialization fileSystemValidationSerialization,
    string searchPattern = "*.*")
    : IFileSystemValidation {

    /// <summary>
    /// Depth of the directory tree to search for invalidated files in parallel.
    /// </summary>
    private const int MaxParallelLevel = 2;

    public async Task<CacheValidationResult<DataRelativePath>> GetInvalidatedContent(FileSystemLink source) {
        var validate = fileSystemValidationSerialization.Validate(source.FullPath);
        if (!validate || !fileSystemValidationSerialization.TryDeserialize(source.FullPath, out var fileSystemCacheData)) {
            var buildCache = await BuildCache(source);
            fileSystemValidationSerialization.Serialize(buildCache, source.FullPath);

            return CacheValidationResult<DataRelativePath>.FullyInvalid(() => {
                fileSystemValidationSerialization.Serialize(buildCache, source.FullPath);
            });
        }

        var invalidatedFiles = await ValidateDirectoryRec(fileSystemCacheData.RootDirectory, source, 1).ToArrayAsync();
        if (invalidatedFiles.Length == 0) return CacheValidationResult<DataRelativePath>.Valid();

        return CacheValidationResult<DataRelativePath>.PartlyInvalid(invalidatedFiles
                .Select(file => new DataRelativePath(source.FileSystem.Path.GetRelativePath(source.FullPath, file.FullPath)))
                .ToArray(),
            () => {
                UpdateCache(fileSystemCacheData, invalidatedFiles, source);
                fileSystemValidationSerialization.Serialize(fileSystemCacheData, source.FullPath);
            });

        async IAsyncEnumerable<FileSystemLink> ValidateDirectoryRec(HashDirectoryCacheData hashDirectoryCache, FileSystemLink directoryPath, int level) {
            var nextLevel = level + 1;

            // Use DFS to search for new or changed files in all subdirectories 
            // For levels up until MaxParallelLevel, we can use run this recursive search in parallel
            if (level > MaxParallelLevel) {
                foreach (var subDirectoryName in directoryPath.EnumerateDirectoryLinks(false)) {
                    await foreach (var fileLink in DirectoryParse(subDirectoryName)) {
                        yield return fileLink;
                    }
                }
            } else {
                var directoryResults = await Task.WhenAll(
                    directoryPath.EnumerateDirectoryLinks(false)
                        .Select(subDirectoryName => Task.Run(() => DirectoryParse(subDirectoryName))));

                foreach (var files in directoryResults) {
                    await foreach (var fileLink in files) {
                        yield return fileLink;
                    }
                }
            }

            // Check all files in this directory and compare their hashes with the cache
            foreach (var fileLink in directoryPath.EnumerateFileLinks(searchPattern, false)) {
                var fileCache = hashDirectoryCache.Files.FirstOrDefault(d => DataRelativePath.PathComparer.Equals(d.Name, fileLink.Name));

                if (fileCache is null) {
                    yield return fileLink;
                } else {
                    if (!fileLink.FileSystem.IsFileHashValid(fileLink.FullPath, fileCache.Hash)) {
                        yield return fileLink;
                    }
                }
            }

            async IAsyncEnumerable<FileSystemLink> DirectoryParse(FileSystemLink subDirectoryPath) {
                var subDirectoryCache = hashDirectoryCache.SubDirectories
                    .FirstOrDefault(d => DataRelativePath.PathComparer.Equals(d.Name, subDirectoryPath.Name));

                if (subDirectoryCache is null) {
                    // No cache exists for this subdirectory, return all new files in this directory as invalid
                    foreach (var filePath in subDirectoryPath.EnumerateFileLinks(searchPattern, true)) {
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

    private void UpdateCache(HashFileSystemCacheData fileSystemCacheData, IEnumerable<FileSystemLink> invalidatedFiles, FileSystemLink rootDirectoryPath) {
        foreach (var fileLink in invalidatedFiles) {
            var relativePath = fileLink.FileSystem.Path.GetRelativePath(rootDirectoryPath.FullPath, fileLink.FullPath);

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

    private async Task<HashFileSystemCacheData> BuildCache(FileSystemLink rootDirectoryPath) {
        var rootDirectory = await BuildDirectoryCache(rootDirectoryPath, 1)
         ?? new HashDirectoryCacheData(rootDirectoryPath.Name, [], []);

        return new HashFileSystemCacheData(rootDirectoryPath.FileSystem.GetHashBytesLength(), rootDirectory);

        async Task<HashDirectoryCacheData?> BuildDirectoryCache(FileSystemLink directoryPath, int level) {
            var nextLevel = level + 1;

            // Build directories
            IList<HashDirectoryCacheData> subDirectories;
            if (level > MaxParallelLevel) {
                var d = new List<HashDirectoryCacheData>();
                foreach (var subDirectoryPath in directoryPath.EnumerateDirectoryLinks(false)) {
                    var hashDirectoryCacheData = await BuildDirectoryCache(subDirectoryPath, nextLevel);
                    if (hashDirectoryCacheData is not null) {
                        d.Add(hashDirectoryCacheData);
                    }
                }
                subDirectories = d;
            } else {
                var hashDirectoryCacheData = await Task.WhenAll(directoryPath.EnumerateDirectoryLinks(false)
                    .Select(dir => Task.Run(() => BuildDirectoryCache(dir, nextLevel))));

                subDirectories = hashDirectoryCacheData.WhereNotNull().ToArray();
            }

            // Build files
            var files = new List<HashFileCacheData>();
            foreach (var filePath in directoryPath.EnumerateFileLinks(searchPattern, false)) {
                var hash = filePath.FileSystem.GetFileHash(filePath.FullPath);

                files.Add(new HashFileCacheData(filePath.Name, hash));
            }

            // Finalize cache
            if (subDirectories.Count == 0 && files.Count == 0) return null;

            return new HashDirectoryCacheData(directoryPath.Name, subDirectories, files);
        }
    }
}
