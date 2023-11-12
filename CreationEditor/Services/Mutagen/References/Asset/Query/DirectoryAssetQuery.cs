using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class DirectoryAssetQuery : IAssetReferenceCacheableQuery<string, string>, IAssetReferenceCacheableValidatableQuery<string, string> {
    private readonly FileAssetParser _fileAssetParser;
    private readonly FileSystemAssetQuery _fileSystemAssetQuery;

    public Version CacheVersion => _fileSystemAssetQuery.CacheVersion;
    public IAssetReferenceSerialization<string, string> Serialization { get; }
    public IInternalCacheValidation<string, string> CacheValidation => _fileSystemAssetQuery.CacheValidation;
    public string QueryName => _fileSystemAssetQuery.QueryName;
    public IDictionary<string, AssetReferenceCache<string, string>> AssetCaches => _fileSystemAssetQuery.AssetCaches;

    public DirectoryAssetQuery(
        Func<IFileAssetParser, FileSystemAssetQuery> fileSystemAssetQuery,
        FileAssetParser fileAssetParser,
        IAssetReferenceSerialization<string, string> serialization) {
        _fileAssetParser = fileAssetParser;
        _fileSystemAssetQuery = fileSystemAssetQuery(fileAssetParser);
        Serialization = serialization;
    }

    public IEnumerable<AssetQueryResult<string>> ParseFile(string source) => _fileAssetParser.ParseFile(source);
    public IEnumerable<AssetQueryResult<string>> ParseAssets(string source) => _fileSystemAssetQuery.ParseAssets(source);
    public bool IsCacheUpToDate(BinaryReader reader, string source) => _fileSystemAssetQuery.IsCacheUpToDate(reader, source);
    public string ReadContextString(BinaryReader reader) => _fileSystemAssetQuery.ReadContextString(reader);
    public IEnumerable<string> ReadUsages(BinaryReader reader, string contextString, int assetUsageCount) => _fileSystemAssetQuery.ReadUsages(reader, contextString, assetUsageCount);
    public void WriteCacheCheck(BinaryWriter writer, string source) => _fileSystemAssetQuery.WriteCacheCheck(writer, source);
    public void WriteContext(BinaryWriter writer, string source) => _fileSystemAssetQuery.WriteContext(writer, source);
    public void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) => _fileSystemAssetQuery.WriteUsages(writer, usages);
}
