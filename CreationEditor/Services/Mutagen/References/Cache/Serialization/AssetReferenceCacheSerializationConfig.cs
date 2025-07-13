using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public sealed class AssetReferenceCacheSerializationConfig : IReferenceCacheSerializationConfig<IDataSource, DataRelativePath> {
    public Version CacheVersion { get; } = new(1, 0);

    public void WriteCacheValidation(BinaryWriter writer, IDataSource source) {}

    public void WriteSource(BinaryWriter writer, IDataSource source) => writer.Write(source.Path);

    public void WriteReferences(BinaryWriter writer, IEnumerable<DataRelativePath> references) {
        foreach (var usage in references) {
            writer.Write(usage.Path);
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, IDataSource source) => true;

    public string ReadSource(BinaryReader reader) => reader.ReadString();

    public IEnumerable<DataRelativePath> ReadReferences(BinaryReader reader, string sourceContextString, int referenceCount) {
        for (var i = 0; i < referenceCount; i++) {
            yield return reader.ReadString();
        }
    }
}
