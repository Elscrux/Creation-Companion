using System.Collections.Concurrent;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.FileSystem.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Assets;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class FileSystemAssetQuery(
    Func<string, IFileSystemValidation> fileSystemValidationFactory,
    ILogger logger,
    IDataSourceService dataSourceService,
    IFileAssetParser fileAssetParser,
    IAssetReferenceSerialization<FileSystemLink, DataRelativePath> serialization)
    : IAssetReferenceCacheableQuery<FileSystemLink, DataRelativePath>, IAssetReferenceCacheableValidatableQuery<FileSystemLink, DataRelativePath> {
    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<FileSystemLink, DataRelativePath> Serialization { get; } = serialization;
    public IInternalCacheValidation<FileSystemLink, DataRelativePath> CacheValidation { get; } = fileSystemValidationFactory(fileAssetParser.FilterPattern);
    public string QueryName => fileAssetParser.Name;
    public IDictionary<FileSystemLink, AssetReferenceCache<FileSystemLink, DataRelativePath>> AssetCaches { get; }
        = new ConcurrentDictionary<FileSystemLink, AssetReferenceCache<FileSystemLink, DataRelativePath>>();

    public string GetName(FileSystemLink source) => source.FullPath;

    public FileSystemLink? ReferenceToSource(DataRelativePath reference) {
        dataSourceService.TryGetFileLink(reference.Path, out var fileLink);
        return fileLink;
    }
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(FileSystemLink path) {
        // For files, just parse the file
        if (path.IsFile) {
            foreach (var result in fileAssetParser.ParseFile(path)) yield return result;

            yield break;
        }

        // For directories, parse all files in the directory and all subdirectories
        foreach (var file in path.EnumerateFileLinks(true)) {
            foreach (var result in fileAssetParser.ParseFile(file)) {
                yield return result;
            }
        }
    }

    public void WriteCacheValidation(BinaryWriter writer, FileSystemLink source) {}

    public void WriteContext(BinaryWriter writer, FileSystemLink source) => writer.Write(source.FullPath);

    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) {
        foreach (var usage in references) {
            writer.Write(usage.Path);
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, FileSystemLink source) => true;

    public string ReadContextString(BinaryReader reader) => reader.ReadString();

    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) {
        for (var i = 0; i < assetReferenceCount; i++) {
            yield return reader.ReadString();
        }
    }
}
