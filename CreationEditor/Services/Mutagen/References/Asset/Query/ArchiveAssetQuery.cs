using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ArchiveAssetQuery(
    IFileSystem fileSystem,
    IArchiveAssetParser archiveAssetParser,
    IAssetReferenceSerialization<string, DataRelativePath> serialization)
    : IAssetReferenceCacheableQuery<string, DataRelativePath> {

    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<string, DataRelativePath> Serialization { get; } = serialization;
    public string QueryName => archiveAssetParser.Name;
    public IDictionary<string, AssetReferenceCache<string, DataRelativePath>> AssetCaches { get; }
        = new ConcurrentDictionary<string, AssetReferenceCache<string, DataRelativePath>>();

    public void WriteCacheValidation(BinaryWriter writer, string source) {
        var hash = fileSystem.GetFileHash(source);
        writer.Write(hash);
    }

    public void WriteContext(BinaryWriter writer, string source) => writer.Write(source);

    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) {
        foreach (var usage in references) {
            writer.Write(usage.Path);
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, string source) {
        var hash = reader.ReadBytes(fileSystem.GetHashBytesLength());
        return fileSystem.IsFileHashValid(source, hash);
    }

    public string ReadContextString(BinaryReader reader) => reader.ReadString();

    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) {
        for (var i = 0; i < assetReferenceCount; i++) {
            yield return reader.ReadString();
        }
    }

    public string? ReferenceToSource(DataRelativePath reference) => null;
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(string archivePath) => archiveAssetParser.ParseAssets(archivePath);
}
