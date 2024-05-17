using CreationEditor.Services.Mutagen.References.Record.Query;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public sealed class ImmutableRecordReferenceCache : IRecordReferenceCache {
    private readonly Dictionary<ModKey, ModReferenceCache> _modCaches;

    public ImmutableRecordReferenceCache(IDictionary<ModKey, ModReferenceCache> caches) {
        _modCaches = new Dictionary<ModKey, ModReferenceCache>(caches);
    }

    public ImmutableRecordReferenceCache(IRecordReferenceQuery recordReferenceQuery) {
        _modCaches = new Dictionary<ModKey, ModReferenceCache>(recordReferenceQuery.ModCaches);
    }

    public ImmutableRecordReferenceCache(ImmutableRecordReferenceCache immutableRecordReferenceCache) {
        _modCaches = new Dictionary<ModKey, ModReferenceCache>(immutableRecordReferenceCache._modCaches);
    }

    public ModReferenceCache? GetModReferenceCache(ModKey modKey) => _modCaches.GetValueOrDefault(modKey);
}
