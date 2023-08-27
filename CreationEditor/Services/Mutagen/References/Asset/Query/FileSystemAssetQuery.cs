using System.IO.Abstractions;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.FileSystem.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class FileSystemAssetQuery : IAssetReferenceCacheableQuery<string, string>, IAssetReferenceCacheableValidatableQuery<string, string> {
    private const string WildcardSearchPattern = "*";

    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IFileAssetParser _fileAssetParser;

    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<string, string> Serialization { get; }
    public IInternalCacheValidation<string, string> CacheValidation { get; }
    public string QueryName => _fileAssetParser.Name;
    public Dictionary<string, AssetReferenceCache<string, string>> AssetCaches { get; } = new();

    public FileSystemAssetQuery(
        Func<string, IFileSystemValidation> fileSystemValidationFactory,
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IFileAssetParser fileAssetParser,
        IAssetReferenceSerialization<string, string> serialization) {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
        _fileAssetParser = fileAssetParser;
        Serialization = serialization;
        CacheValidation = fileSystemValidationFactory(fileAssetParser.FilterPattern);
    }

    public IEnumerable<AssetQueryResult<string>> ParseAssets(string path) {
        var isFilePath = _fileSystem.Path.HasExtension(path);

        // If the path is not rooted, combine it with the data directory
        if (!_fileSystem.Path.IsPathRooted(path)) {
            path = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, path);
        }

        // For files, just parse the file
        if (isFilePath) {
            foreach (var result in _fileAssetParser.ParseFile(path)) yield return result;

            yield break;
        }

        // For directories, parse all files in the directory and all subdirectories
        foreach (var file in _fileSystem.Directory.EnumerateFiles(path, WildcardSearchPattern, SearchOption.AllDirectories)) {
            foreach (var result in _fileAssetParser.ParseFile(file)) {
                yield return result;
            }
        }
    }

    public void WriteCacheCheck(BinaryWriter writer, string source) {}

    public void WriteContext(BinaryWriter writer, string source) => writer.Write(source);

    public void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) {
        foreach (var usage in usages) {
            writer.Write(usage);
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, string source) => true;

    public string ReadContextString(BinaryReader reader) => reader.ReadString();

    public IEnumerable<string> ReadUsages(BinaryReader reader, string context, int count) {
        for (var i = 0; i < count; i++) {
            yield return reader.ReadString();
        }
    }
}
