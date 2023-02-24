using CreationEditor.Services.Mutagen.References.Query;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Cache;

public class ImmutableReferenceCache : IReferenceCache {
    private readonly IReadOnlyDictionary<ModKey, ReferenceQuery.ModReferenceCache> _modCaches;

    public ImmutableReferenceCache(IReferenceQuery referenceQuery, IReadOnlyList<IModGetter> mods) {
        referenceQuery.LoadModReferences(mods);
        _modCaches = new Dictionary<ModKey, ReferenceQuery.ModReferenceCache>(referenceQuery.ModCaches);
    }

    public ImmutableReferenceCache(ImmutableReferenceCache immutableReferenceCache) {
        _modCaches = new Dictionary<ModKey, ReferenceQuery.ModReferenceCache>(immutableReferenceCache._modCaches);
    }

    /// <summary>
    /// Returns references of one form key in a mod
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="modOrder">list of mods to get references from, earlier items are prioritized</param>
    /// <returns>form links of references</returns>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> modOrder) {
        foreach (var mod in modOrder) {
            if (!_modCaches.TryGetValue(mod.ModKey, out var modReferenceCache)
             || !modReferenceCache.Cache.TryGetValue(formKey, out var references)) continue;

            foreach (var reference in references) {
                var containingMod = modOrder.FirstOrDefault(m => _modCaches.TryGetValue(m.ModKey, out var referenceCache) && referenceCache.FormKeys.Contains(reference.FormKey));
                if (containingMod?.ModKey == mod.ModKey) {
                    yield return reference;
                }
            }
        }
    }

    /// <summary>
    /// Returns references of one form key in a mod
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="mod">mod to get references from</param>
    /// <returns>form links of references</returns>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IModGetter mod) {
        if (!_modCaches.TryGetValue(mod.ModKey, out var modReferenceCache)
         || !modReferenceCache.Cache.TryGetValue(formKey, out var references)) yield break;

        foreach (var reference in references) {
            yield return reference;
        }
    }
}
