using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public sealed record ModReferenceCache(Dictionary<FormKey, HashSet<IFormLinkIdentifier>> Cache, HashSet<FormKey> FormKeys) {
    public static ModReferenceCache operator +(ModReferenceCache a, ModReferenceCache b) {
        var newRefCache = new ModReferenceCache(a.Cache, a.FormKeys);

        foreach (var formKey in b.FormKeys) {
            newRefCache.FormKeys.Add(formKey);
        }

        foreach (var (formKey, references) in b.Cache) {
            var existingReferences = newRefCache.Cache.GetOrAdd(formKey);

            foreach (var reference in references) {
                existingReferences.Add(reference);
            }
        }

        return newRefCache;
    }
}
