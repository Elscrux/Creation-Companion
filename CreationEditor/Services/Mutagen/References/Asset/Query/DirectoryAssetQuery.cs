using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class DirectoryAssetQuery(
    Func<IFileAssetParser, FileSystemAssetQuery> fileSystemAssetQuery,
    FileAssetParser fileAssetParser,
    IAssetReferenceSerialization<FileSystemLink, DataRelativePath> serialization)
    : IAssetReferenceCacheableQuery<FileSystemLink, DataRelativePath>, IAssetReferenceCacheableValidatableQuery<FileSystemLink, DataRelativePath> {

    private readonly FileSystemAssetQuery _fileSystemAssetQuery = fileSystemAssetQuery(fileAssetParser);

    public Version CacheVersion => _fileSystemAssetQuery.CacheVersion;
    public IAssetReferenceSerialization<FileSystemLink, DataRelativePath> Serialization { get; } = serialization;
    public IInternalCacheValidation<FileSystemLink, DataRelativePath> CacheValidation => _fileSystemAssetQuery.CacheValidation;
    public string QueryName => _fileSystemAssetQuery.QueryName;
    public IDictionary<FileSystemLink, AssetReferenceCache<FileSystemLink, DataRelativePath>> AssetCaches => _fileSystemAssetQuery.AssetCaches;

    public string GetName(FileSystemLink source) => source.FullPath;
    public FileSystemLink? ReferenceToSource(DataRelativePath reference) => _fileSystemAssetQuery.ReferenceToSource(reference);
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(FileSystemLink source) => fileAssetParser.ParseFile(source);
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(FileSystemLink source) => _fileSystemAssetQuery.ParseAssets(source);
    public bool IsCacheUpToDate(BinaryReader reader, FileSystemLink source) => _fileSystemAssetQuery.IsCacheUpToDate(reader, source);
    public string ReadContextString(BinaryReader reader) => _fileSystemAssetQuery.ReadContextString(reader);
    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) =>
        _fileSystemAssetQuery.ReadReferences(reader, contextString, assetReferenceCount);
    public void WriteCacheValidation(BinaryWriter writer, FileSystemLink source) => _fileSystemAssetQuery.WriteCacheValidation(writer, source);
    public void WriteContext(BinaryWriter writer, FileSystemLink source) => _fileSystemAssetQuery.WriteContext(writer, source);
    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) =>
        _fileSystemAssetQuery.WriteReferences(writer, references);
}
