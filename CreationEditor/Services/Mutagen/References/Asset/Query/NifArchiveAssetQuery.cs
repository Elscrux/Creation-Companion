using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class NifArchiveAssetQuery(
    Func<IArchiveAssetParser, ArchiveAssetQuery> archiveAssetQuery,
    NifArchiveAssetParser nifArchiveAssetParser,
    IAssetReferenceSerialization<string, DataRelativePath> serialization)
    : IAssetReferenceCacheableQuery<string, DataRelativePath> {

    private readonly ArchiveAssetQuery _archiveAssetQuery = archiveAssetQuery(nifArchiveAssetParser);

    public Version CacheVersion => _archiveAssetQuery.CacheVersion;
    public IAssetReferenceSerialization<string, DataRelativePath> Serialization { get; } = serialization;
    public string QueryName => _archiveAssetQuery.QueryName;
    public IDictionary<string, AssetReferenceCache<string, DataRelativePath>> AssetCaches => _archiveAssetQuery.AssetCaches;

    public string? ReferenceToSource(DataRelativePath reference) => _archiveAssetQuery.ReferenceToSource(reference);
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(string source) => _archiveAssetQuery.ParseAssets(source);
    public bool IsCacheUpToDate(BinaryReader reader, string source) => _archiveAssetQuery.IsCacheUpToDate(reader, source);
    public string ReadContextString(BinaryReader reader) => _archiveAssetQuery.ReadContextString(reader);
    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) =>
        _archiveAssetQuery.ReadReferences(reader, contextString, assetReferenceCount);
    public void WriteCacheValidation(BinaryWriter writer, string source) => _archiveAssetQuery.WriteCacheValidation(writer, source);
    public void WriteContext(BinaryWriter writer, string source) => _archiveAssetQuery.WriteContext(writer, source);
    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) =>
        _archiveAssetQuery.WriteReferences(writer, references);
}
