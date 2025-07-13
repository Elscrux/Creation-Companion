using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public sealed class ModAssetReferenceCacheSerializationConfig(
    IDataSourceService dataSourceService,
    IMutagenTypeProvider mutagenTypeProvider)
    : IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> {
    public Version CacheVersion { get; } = new(1, 0);

    public void WriteCacheValidation(BinaryWriter writer, IModGetter mod) {
        if (!dataSourceService.TryGetFileLink(mod.ModKey.FileName.String, out var modLink)) return;
        if (!modLink.Exists()) return;

        var hash = modLink.FileSystem.GetFileHash(modLink.FullPath);
        writer.Write(hash);
    }

    public void WriteSource(BinaryWriter writer, IModGetter source) {
        // Write game
        writer.Write(mutagenTypeProvider.GetGameName(source));
    }

    public void WriteReferences(BinaryWriter writer, IEnumerable<IFormLinkIdentifier> references) {
        foreach (var usage in references) {
            writer.Write(usage.FormKey.ToString());
            writer.Write(mutagenTypeProvider.GetTypeName(usage));
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, IModGetter source) {
        if (!dataSourceService.TryGetFileLink(source.ModKey.FileName.String, out var modLink)) return false;
        if (!modLink.Exists()) return false;

        // Read hash in cache
        var hash = reader.ReadBytes(modLink.FileSystem.GetHashBytesLength());

        // Validate hash
        return modLink.FileSystem.IsFileHashValid(modLink.FullPath, hash);
    }

    public string ReadSource(BinaryReader reader) {
        // Read game string
        var game = reader.ReadString();
        return game;
    }

    public IEnumerable<IFormLinkIdentifier> ReadReferences(BinaryReader reader, string sourceContextString, int referenceCount) {
        for (var i = 0; i < referenceCount; i++) {
            var referenceFormKey = FormKey.Factory(reader.ReadString());
            var typeString = reader.ReadString();
            var type = mutagenTypeProvider.GetType(sourceContextString, typeString);

            yield return new FormLinkInformation(referenceFormKey, type);
        }
    }
}
