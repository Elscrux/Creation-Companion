using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class RecordAssetReferenceQueryConfig(
    ReferenceCacheBuilder referenceCacheBuilder,
    ModAssetSerializableQuery modAssetSerializableQuery,
    AssetReferenceCacheSerialization<IModGetter, IFormLinkIdentifier> serialization)
    : IReferenceQueryConfig<IModGetter, RecordModPair, AssetReferenceCache<IFormLinkIdentifier>, IAssetLinkGetter> {
    public IEqualityComparer<IModGetter> EqualityComparer => ModComparer.Instance;
    public bool CanGetLinksFromDeletedElement => true;
    public string Name => modAssetSerializableQuery.Name;

    public Task<AssetReferenceCache<IFormLinkIdentifier>> BuildCache(IModGetter source) {
        return referenceCacheBuilder.BuildCache(source, modAssetSerializableQuery, serialization);
    }

    public IEnumerable<IAssetLinkGetter> GetLinks(RecordModPair element) {
        return modAssetSerializableQuery.ParseRecord(element.Record);
    }

    public object GetSourceKey(IModGetter source) => source.ModKey;
}
