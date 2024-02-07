using System.Collections.Concurrent;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public sealed class MutableRecordReferenceCache(
    IDictionary<ModKey, ModReferenceCache> mutableModReferenceCaches,
    ImmutableRecordReferenceCache? immutableReferenceCache = null)
    : IRecordReferenceCache {

    private readonly ConcurrentDictionary<ModKey, ModReferenceCache> _mutableModReferenceCaches = new(mutableModReferenceCaches);

    public bool AddRecord(IMod mod, IMajorRecordGetter record) {
        return _mutableModReferenceCaches[mod.ModKey].FormKeys.Add(record.FormKey);
    }

    public bool RemoveReference(IMod mod, FormKey formKey, IFormLinkIdentifier oldReference) {
        // when the record was not part of the MUTABLE MOD before we need to reevaluate all old form from the other one too  

        return _mutableModReferenceCaches[mod.ModKey].Cache.TryGetValue(formKey, out var references)
         && references.Remove(oldReference);
    }

    public void RemoveReferences(IMod mod, FormKey formKey, IEnumerable<IFormLinkIdentifier> oldReferences) {
        if (!_mutableModReferenceCaches[mod.ModKey].Cache.TryGetValue(formKey, out var references)) return;

        foreach (var oldReference in oldReferences) {
            references.Remove(oldReference);
        }
    }

    public bool AddReference(IMod mod, FormKey formKey, IFormLinkIdentifier newReference) {
        var references = _mutableModReferenceCaches[mod.ModKey].Cache.GetOrAdd(formKey);

        return references.Add(newReference);
    }

    public void AddReferences(IMod mod, FormKey formKey, IEnumerable<IFormLinkIdentifier> newReferences) {
        var references = _mutableModReferenceCaches[mod.ModKey].Cache.GetOrAdd(formKey);
        foreach (var newReference in newReferences) {
            references.Add(newReference);
        }
    }

    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> modOrder) {
        foreach (var mod in modOrder) {
            var modReferenceCache = GetModReferenceCache(mod.ModKey);
            if (!modReferenceCache.Cache.TryGetValue(formKey, out var references)) continue;

            foreach (var reference in references) {
                var containingMod = modOrder.FirstOrDefault(m => GetModReferenceCache(m.ModKey).FormKeys.Contains(reference.FormKey));
                if (containingMod?.ModKey == mod.ModKey) {
                    yield return reference;
                }
            }
        }
    }

    private ModReferenceCache GetModReferenceCache(ModKey modKey) {
        if (_mutableModReferenceCaches.TryGetValue(modKey, out var modReferenceCache)) return modReferenceCache;
        if (immutableReferenceCache is not null) return immutableReferenceCache.GetModReferenceCache(modKey);

        throw new KeyNotFoundException($"Mod {modKey} not found in the reference cache");
    }
}
