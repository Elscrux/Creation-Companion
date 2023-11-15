using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ArchiveAssetQuery(
    IFileSystem fileSystem,
    IArchiveAssetParser archiveAssetParser,
    IAssetReferenceSerialization<string, string> serialization)
    : IAssetReferenceCacheableQuery<string, string> {

    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<string, string> Serialization { get; } = serialization;
    public string QueryName => archiveAssetParser.Name;
    public IDictionary<string, AssetReferenceCache<string, string>> AssetCaches { get; }
        = new ConcurrentDictionary<string, AssetReferenceCache<string, string>>();

    public void WriteCacheCheck(BinaryWriter writer, string source) {
        var hash = fileSystem.GetFileHash(source);
        writer.Write(hash);
    }

    public void WriteContext(BinaryWriter writer, string source) => writer.Write(source);

    public void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) {
        foreach (var usage in usages) {
            writer.Write(usage);
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, string source) {
        var hash = reader.ReadBytes(fileSystem.GetHashBytesLength());
        return fileSystem.IsFileHashValid(source, hash);
    }

    public string ReadContextString(BinaryReader reader) => reader.ReadString();

    public IEnumerable<string> ReadUsages(BinaryReader reader, string contextString, int assetUsageCount) {
        for (var i = 0; i < assetUsageCount; i++) {
            yield return reader.ReadString();
        }
    }

    public IEnumerable<AssetQueryResult<string>> ParseAssets(string archivePath) => archiveAssetParser.ParseAssets(archivePath);
}
