using CreationEditor.Services.Mutagen.References.Record.Query;
using Mutagen.Bethesda.Plugins;
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

    public async Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IReadOnlyList<IMod> mutableMods, IReadOnlyList<IModGetter> mods) {
        var mutableCaches = await GetMutableModReferenceCaches(mutableMods);
        var immutableRecordReferenceCache = await GetImmutableRecordReferenceCache(mods.Except(mutableMods).ToList());
        return new MutableRecordReferenceCache(mutableCaches, immutableRecordReferenceCache);
    }

    public async Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IReadOnlyList<IMod> mutableMods, ImmutableRecordReferenceCache? immutableRecordReferenceCache = null) {
        var mutableCaches = await GetMutableModReferenceCaches(mutableMods);
        var duplicateImmutableRecordReferenceCache =
            immutableRecordReferenceCache is null
                ? null
                : await GetImmutableRecordReferenceCache(immutableRecordReferenceCache);

        return new MutableRecordReferenceCache(mutableCaches, duplicateImmutableRecordReferenceCache);
    }

    private async Task<Dictionary<ModKey, ModReferenceCache>> GetMutableModReferenceCaches(IReadOnlyList<IMod> mutableMods) {
        await recordReferenceQuery.LoadModReferences(mutableMods);
        return mutableMods.Select(x => x.ModKey).ToDictionary(m => m, m => recordReferenceQuery.ModCaches[m]);
    }
}
