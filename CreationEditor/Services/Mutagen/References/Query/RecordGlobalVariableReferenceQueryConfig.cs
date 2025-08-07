using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class RecordGlobalVariableReferenceQueryConfig(
    ReferenceCacheBuilder referenceCacheBuilder,
    RecordGlobalVariableReferenceQuery globalVariableReferenceQuery,
    Func<IReadOnlyList<string>, DictionaryReferenceCacheSerialization<IModGetter, DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IFormLinkIdentifier>> serializationFactory)
    : IReferenceQueryConfig<IModGetter, RecordModPair, DictionaryReferenceCache<string, IFormLinkIdentifier>, string> {
    private readonly DictionaryReferenceCacheSerialization<IModGetter, DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IFormLinkIdentifier> _serialization =
        serializationFactory(["References"]);

    public bool CanGetLinksFromDeletedElement => true;
    public string Name => globalVariableReferenceQuery.Name;

    public Task<DictionaryReferenceCache<string, IFormLinkIdentifier>> BuildCache(IModGetter source) {
        return referenceCacheBuilder.BuildCache(source, globalVariableReferenceQuery, _serialization);
    }

    public IEnumerable<string> GetLinks(RecordModPair element) {
        return globalVariableReferenceQuery.ParseRecord(element.Record);
    }
}
