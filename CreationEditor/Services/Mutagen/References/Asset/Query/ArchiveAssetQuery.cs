using System.IO.Abstractions;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ArchiveAssetQuery : IAssetReferenceCacheableQuery<string, string> {
    private readonly IFileSystem _fileSystem;
    private readonly IArchiveAssetParser _archiveAssetParser;

    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<string, string> Serialization { get; }
    public IInternalCacheValidation<string, string>? CacheValidation => null;
    public string QueryName => _archiveAssetParser.Name;
    public Dictionary<string, AssetReferenceCache<string, string>> AssetCaches { get; } = new();

    public ArchiveAssetQuery(
        IFileSystem fileSystem,
        IArchiveAssetParser archiveAssetParser,
        IAssetReferenceSerialization<string, string> serialization) {
        _fileSystem = fileSystem;
        _archiveAssetParser = archiveAssetParser;
        Serialization = serialization;
    }

    public void WriteCacheCheck(BinaryWriter writer, string source) {
        var hash = _fileSystem.GetFileHash(source);
        writer.Write(hash);
    }

    public void WriteContext(BinaryWriter writer, string source) => writer.Write(source);

    public void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) {
        foreach (var usage in usages) {
            writer.Write(usage);
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, string source) {
        var hash = reader.ReadBytes(_fileSystem.GetHashBytesLength());
        return _fileSystem.IsFileHashValid(source, hash);
    }

    public string ReadContextString(BinaryReader reader) => reader.ReadString();

    public IEnumerable<string> ReadUsages(BinaryReader reader, string contextString, int assetUsageCount) {
        for (var i = 0; i < assetUsageCount; i++) {
            yield return reader.ReadString();
        }
    }

    public IEnumerable<AssetQueryResult<string>> ParseAssets(string archivePath) => _archiveAssetParser.ParseAssets(archivePath);
}
