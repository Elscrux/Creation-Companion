using CreationEditor.Services.Mutagen.References.Record.Query;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public sealed class ImmutableRecordReferenceCache : IRecordReferenceCache {
    private readonly IReadOnlyDictionary<ModKey, ModReferenceCache> _modCaches;

    internal ImmutableRecordReferenceCache(IRecordReferenceQuery recordReferenceQuery) {
        _modCaches = new Dictionary<ModKey, ModReferenceCache>(recordReferenceQuery.ModCaches);
    }

    internal ImmutableRecordReferenceCache(ImmutableRecordReferenceCache immutableRecordReferenceCache) {
        _modCaches = new Dictionary<ModKey, ModReferenceCache>(immutableRecordReferenceCache._modCaches);
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
