namespace CreationEditor.Services.FileSystem.Validation;

public sealed record HashFileSystemCacheData(
    int HashLength,
    HashDirectoryCacheData RootDirectory);

public sealed record HashDirectoryCacheData(
    string Name,
    IList<HashDirectoryCacheData> SubDirectories,
    IList<HashFileCacheData> Files);

public sealed record HashFileCacheData(
    string Name,
    byte[] Hash);
