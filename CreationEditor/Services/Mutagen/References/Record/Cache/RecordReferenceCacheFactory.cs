using CreationEditor.Services.Mutagen.References.Record.Query;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public sealed class RecordReferenceCacheFactory(IRecordReferenceQuery recordReferenceQuery) : IRecordReferenceCacheFactory {
    public async Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(IReadOnlyList<IModGetter> mods) {
        await recordReferenceQuery.LoadModReferences(mods);
        return new ImmutableRecordReferenceCache(recordReferenceQuery);
    }

    public Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(ImmutableRecordReferenceCache immutableRecordReferenceCache) {
        return Task.FromResult(new ImmutableRecordReferenceCache(immutableRecordReferenceCache));
    }

    public async Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IMod mutableMod, IReadOnlyList<IModGetter> mods) {
        recordReferenceQuery.LoadModReferences(mutableMod);
        var mutableModReferenceCache = recordReferenceQuery.ModCaches[mutableMod.ModKey];
        var immutableRecordReferenceCache = await GetImmutableRecordReferenceCache(mods);
        return new MutableRecordReferenceCache(mutableMod, mutableModReferenceCache, immutableRecordReferenceCache);
    }

    public async Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IMod mutableMod, ImmutableRecordReferenceCache? immutableRecordReferenceCache = null) {
        recordReferenceQuery.LoadModReferences(mutableMod);
        var mutableModReferenceCache = recordReferenceQuery.ModCaches[mutableMod.ModKey];
        var duplicateImmutableRecordReferenceCache =
            immutableRecordReferenceCache is null
                ? null
                : await GetImmutableRecordReferenceCache(immutableRecordReferenceCache);

        return new MutableRecordReferenceCache(mutableMod, mutableModReferenceCache, duplicateImmutableRecordReferenceCache);
    }
}
