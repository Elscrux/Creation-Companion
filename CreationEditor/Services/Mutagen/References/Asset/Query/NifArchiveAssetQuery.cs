using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class NifArchiveAssetQuery(
    Func<IArchiveAssetParser, ArchiveAssetQuery> archiveAssetQuery,
    NifArchiveAssetParser nifArchiveAssetParser,
    IAssetReferenceSerialization<FileSystemLink, DataRelativePath> serialization)
    : IAssetReferenceCacheableQuery<FileSystemLink, DataRelativePath> {

    private readonly ArchiveAssetQuery _archiveAssetQuery = archiveAssetQuery(nifArchiveAssetParser);

    public Version CacheVersion => _archiveAssetQuery.CacheVersion;
    public IAssetReferenceSerialization<FileSystemLink, DataRelativePath> Serialization { get; } = serialization;
    public string QueryName => _archiveAssetQuery.QueryName;
    public IDictionary<FileSystemLink, AssetReferenceCache<FileSystemLink, DataRelativePath>> AssetCaches => _archiveAssetQuery.AssetCaches;

    public string GetName(FileSystemLink source) => source.FullPath;
    public FileSystemLink? ReferenceToSource(DataRelativePath reference) => _archiveAssetQuery.ReferenceToSource(reference);
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(FileSystemLink source) => _archiveAssetQuery.ParseAssets(source);
    public bool IsCacheUpToDate(BinaryReader reader, FileSystemLink source) => _archiveAssetQuery.IsCacheUpToDate(reader, source);
    public string ReadContextString(BinaryReader reader) => _archiveAssetQuery.ReadContextString(reader);
    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) =>
        _archiveAssetQuery.ReadReferences(reader, contextString, assetReferenceCount);
    public void WriteCacheValidation(BinaryWriter writer, FileSystemLink source) => _archiveAssetQuery.WriteCacheValidation(writer, source);
    public void WriteContext(BinaryWriter writer, FileSystemLink source) => _archiveAssetQuery.WriteContext(writer, source);
    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) =>
        _archiveAssetQuery.WriteReferences(writer, references);
}
