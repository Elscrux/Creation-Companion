using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public interface IRecordReferenceCache {
    /// <summary>
    /// Returns references of one form key in a mod
    /// </summary>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="modOrder">list of mods to get references from, earlier items are prioritized</param>
    /// <returns>form links of references</returns>

    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> modOrder) {
        foreach (var modKey in modOrder.Select(x => x.ModKey)) {
            var modReferenceCache = GetModReferenceCache(modKey);
            if (modReferenceCache is null || !modReferenceCache.Cache.TryGetValue(formKey, out var references)) continue;

            foreach (var reference in references) {
                // Skip references that are overridden by another mod
                var containingMod = modOrder.FirstOrDefault(m => GetModReferenceCache(m.ModKey) is {} cache && cache.FormKeys.Contains(reference.FormKey));
                if (containingMod?.ModKey != modKey) continue;

                yield return reference;
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
        if (GetModReferenceCache(mod.ModKey) is not {} cache
         || !cache.Cache.TryGetValue(formKey, out var references)) yield break;

        foreach (var reference in references) {
            yield return reference;
        }
    }

    protected ModReferenceCache? GetModReferenceCache(ModKey modKey);
}
