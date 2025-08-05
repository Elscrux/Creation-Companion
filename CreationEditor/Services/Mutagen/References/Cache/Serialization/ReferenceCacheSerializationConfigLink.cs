using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public class ReferenceCacheSerializationConfigLink(IMutagenTypeProvider mutagenTypeProvider) : IReferenceCacheSerializationConfigLink<IModGetter, string, IFormLinkIdentifier> {
    public Version CacheVersion { get; } = new(1, 0);
    public bool IsCacheUpToDate(BinaryReader reader, IModGetter source) => true;
    public string ReadSource(BinaryReader reader) => reader.ReadString();
    public IEnumerable<IFormLinkIdentifier> ReadReferences(BinaryReader reader, string sourceContextString, int referenceCount) {
        for (var i = 0; i < referenceCount; i++) {
            var referenceFormKey = FormKey.Factory(reader.ReadString());
            var typeString = reader.ReadString();
            var type = mutagenTypeProvider.GetType(sourceContextString, typeString);
            yield return new FormLinkInformation(referenceFormKey, type);
        }
    }
    public void WriteCacheValidation(BinaryWriter writer, IModGetter source) {
        // No cache validation needed for this type
    }
    public void WriteSource(BinaryWriter writer, IModGetter source) {
        writer.Write(source.GameRelease.ToCategory().ToString());
    }
    public void WriteReferences(BinaryWriter writer, IEnumerable<IFormLinkIdentifier> references) {
        foreach (var reference in references) {
            writer.Write(reference.FormKey.ToString());
            writer.Write(mutagenTypeProvider.GetTypeName(reference));
        }
    }
    public string ReadLink(BinaryReader reader) => reader.ReadString();
    public void WriteLink(BinaryWriter writer, string link) => writer.Write(link);
}
