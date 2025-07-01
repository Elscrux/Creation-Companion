using System.Collections.Concurrent;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ArchiveAssetQuery(
    IArchiveAssetParser archiveAssetParser,
    IAssetReferenceSerialization<FileSystemLink, DataRelativePath> serialization)
    : IAssetReferenceCacheableQuery<FileSystemLink, DataRelativePath> {

    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<FileSystemLink, DataRelativePath> Serialization { get; } = serialization;
    public string QueryName => archiveAssetParser.Name;
    public IDictionary<FileSystemLink, AssetReferenceCache<FileSystemLink, DataRelativePath>> AssetCaches { get; }
        = new ConcurrentDictionary<FileSystemLink, AssetReferenceCache<FileSystemLink, DataRelativePath>>();

    public string GetName(FileSystemLink source) => source.FullPath;

    public void WriteCacheValidation(BinaryWriter writer, FileSystemLink source) {
        var hash = source.FileSystem.GetFileHash(source.FullPath);
        writer.Write(hash);
    }

    public void WriteContext(BinaryWriter writer, FileSystemLink source) => writer.Write(source.FullPath);

    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) {
        foreach (var usage in references) {
            writer.Write(usage.Path);
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, FileSystemLink source) {
        var hash = reader.ReadBytes(source.FileSystem.GetHashBytesLength());
        return source.FileSystem.IsFileHashValid(source.FullPath, hash);
    }

    public string ReadContextString(BinaryReader reader) => reader.ReadString();

    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) {
        for (var i = 0; i < assetReferenceCount; i++) {
            yield return reader.ReadString();
        }
    }

    public FileSystemLink? ReferenceToSource(DataRelativePath reference) => null;
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(FileSystemLink archivePath) => archiveAssetParser.ParseAssets(archivePath);
}
