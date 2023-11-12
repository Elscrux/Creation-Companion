using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class NifArchiveAssetQuery : IAssetReferenceCacheableQuery<string, string> {
    private readonly ArchiveAssetQuery _archiveAssetQuery;

    public Version CacheVersion => _archiveAssetQuery.CacheVersion;
    public IAssetReferenceSerialization<string, string> Serialization { get; }
    public string QueryName => _archiveAssetQuery.QueryName;
    public IDictionary<string, AssetReferenceCache<string, string>> AssetCaches => _archiveAssetQuery.AssetCaches;

    public NifArchiveAssetQuery(
        Func<IArchiveAssetParser, ArchiveAssetQuery> archiveAssetQuery,
        NifArchiveAssetParser nifArchiveAssetParser,
        IAssetReferenceSerialization<string, string> serialization) {
        Serialization = serialization;
        _archiveAssetQuery = archiveAssetQuery(nifArchiveAssetParser);
    }

    public IEnumerable<AssetQueryResult<string>> ParseAssets(string source) => _archiveAssetQuery.ParseAssets(source);
    public bool IsCacheUpToDate(BinaryReader reader, string source) => _archiveAssetQuery.IsCacheUpToDate(reader, source);
    public string ReadContextString(BinaryReader reader) => _archiveAssetQuery.ReadContextString(reader);
    public IEnumerable<string> ReadUsages(BinaryReader reader, string contextString, int assetUsageCount) => _archiveAssetQuery.ReadUsages(reader, contextString, assetUsageCount);
    public void WriteCacheCheck(BinaryWriter writer, string source) => _archiveAssetQuery.WriteCacheCheck(writer, source);
    public void WriteContext(BinaryWriter writer, string source) => _archiveAssetQuery.WriteContext(writer, source);
    public void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) => _archiveAssetQuery.WriteUsages(writer, usages);
}
