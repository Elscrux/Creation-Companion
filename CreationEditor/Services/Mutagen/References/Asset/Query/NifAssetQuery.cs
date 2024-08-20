using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class NifAssetQuery(
    Func<IFileAssetParser, FileSystemAssetQuery> fileSystemAssetQuery,
    NifFileAssetParser nifFileAssetParser,
    IAssetReferenceSerialization<string, DataRelativePath> serialization)
    : IAssetReferenceCacheableQuery<string, DataRelativePath>, IAssetReferenceCacheableValidatableQuery<string, DataRelativePath> {

    private readonly FileSystemAssetQuery _fileSystemAssetQuery = fileSystemAssetQuery(nifFileAssetParser);

    public Version CacheVersion => _fileSystemAssetQuery.CacheVersion;
    public IAssetReferenceSerialization<string, DataRelativePath> Serialization { get; } = serialization;
    public IInternalCacheValidation<string, DataRelativePath> CacheValidation => _fileSystemAssetQuery.CacheValidation;
    public string QueryName => _fileSystemAssetQuery.QueryName;
    public IDictionary<string, AssetReferenceCache<string, DataRelativePath>> AssetCaches => _fileSystemAssetQuery.AssetCaches;

    public string ReferenceToSource(DataRelativePath reference) => _fileSystemAssetQuery.ReferenceToSource(reference);
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(string source) => nifFileAssetParser.ParseFile(source);
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(string source) => _fileSystemAssetQuery.ParseAssets(source);
    public bool IsCacheUpToDate(BinaryReader reader, string source) => _fileSystemAssetQuery.IsCacheUpToDate(reader, source);
    public string ReadContextString(BinaryReader reader) => _fileSystemAssetQuery.ReadContextString(reader);
    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) =>
        _fileSystemAssetQuery.ReadReferences(reader, contextString, assetReferenceCount);
    public void WriteCacheValidation(BinaryWriter writer, string source) => _fileSystemAssetQuery.WriteCacheValidation(writer, source);
    public void WriteContext(BinaryWriter writer, string source) => _fileSystemAssetQuery.WriteContext(writer, source);
    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) =>
        _fileSystemAssetQuery.WriteReferences(writer, references);
}
