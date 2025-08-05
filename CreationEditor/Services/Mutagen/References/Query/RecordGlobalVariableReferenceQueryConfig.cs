using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class RecordGlobalVariableReferenceQueryConfig(
    ReferenceCacheBuilder referenceCacheBuilder,
    RecordGlobalVariableReferenceQuery globalVariableReferenceQuery,
    DictionaryReferenceCacheSerialization<IModGetter, DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IFormLinkIdentifier> serialization)
    : IReferenceQueryConfig<IModGetter, RecordModPair, DictionaryReferenceCache<string, IFormLinkIdentifier>, string> {
    public bool CanGetLinksFromDeletedElement => true;
    public string Name => globalVariableReferenceQuery.Name;

    public Task<DictionaryReferenceCache<string, IFormLinkIdentifier>> BuildCache(IModGetter source) {
        return referenceCacheBuilder.BuildCache(source, globalVariableReferenceQuery, serialization);
    }

    public IEnumerable<string> GetLinks(RecordModPair element) {
        return globalVariableReferenceQuery.ParseRecord(element.Record);
    }
}
