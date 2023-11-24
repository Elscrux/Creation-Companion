using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class DirectoryAssetQuery(
    Func<IFileAssetParser, FileSystemAssetQuery> fileSystemAssetQuery,
    FileAssetParser fileAssetParser,
    IAssetReferenceSerialization<string, string> serialization)
    : IAssetReferenceCacheableQuery<string, string>, IAssetReferenceCacheableValidatableQuery<string, string> {

    private readonly FileSystemAssetQuery _fileSystemAssetQuery = fileSystemAssetQuery(fileAssetParser);

    public Version CacheVersion => _fileSystemAssetQuery.CacheVersion;
    public IAssetReferenceSerialization<string, string> Serialization { get; } = serialization;
    public IInternalCacheValidation<string, string> CacheValidation => _fileSystemAssetQuery.CacheValidation;
    public string QueryName => _fileSystemAssetQuery.QueryName;
    public IDictionary<string, AssetReferenceCache<string, string>> AssetCaches => _fileSystemAssetQuery.AssetCaches;

    public IEnumerable<AssetQueryResult<string>> ParseFile(string source) => fileAssetParser.ParseFile(source);
    public IEnumerable<AssetQueryResult<string>> ParseAssets(string source) => _fileSystemAssetQuery.ParseAssets(source);
    public bool IsCacheUpToDate(BinaryReader reader, string source) => _fileSystemAssetQuery.IsCacheUpToDate(reader, source);
    public string ReadContextString(BinaryReader reader) => _fileSystemAssetQuery.ReadContextString(reader);
    public IEnumerable<string> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) => _fileSystemAssetQuery.ReadReferences(reader, contextString, assetReferenceCount);
    public void WriteCacheValidation(BinaryWriter writer, string source) => _fileSystemAssetQuery.WriteCacheValidation(writer, source);
    public void WriteContext(BinaryWriter writer, string source) => _fileSystemAssetQuery.WriteContext(writer, source);
    public void WriteReferences(BinaryWriter writer, IEnumerable<string> references) => _fileSystemAssetQuery.WriteReferences(writer, references);
}
