using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class RecordReferenceQueryConfig(
    ReferenceCacheBuilder referenceCacheBuilder,
    RecordReferenceQuery recordReferenceQuery,
    RecordReferenceCacheSerialization serialization)
    : IReferenceQueryConfig<IModGetter, RecordModPair, RecordReferenceCache, IFormLinkIdentifier> {
    public IEqualityComparer<IModGetter> EqualityComparer => ModComparer.Instance;
    public bool CanGetLinksFromDeletedElement => true;
    public string Name => recordReferenceQuery.Name;

    public Task<RecordReferenceCache> BuildCache(IModGetter source) {
        return referenceCacheBuilder.BuildCache(source, recordReferenceQuery, serialization);
    }

    public IEnumerable<IFormLinkIdentifier> GetLinks(RecordModPair element) {
        return RecordReferenceQuery.ParseRecord(element.Record);
    }

    public object GetSourceKey(IModGetter source) => source.ModKey;
}
