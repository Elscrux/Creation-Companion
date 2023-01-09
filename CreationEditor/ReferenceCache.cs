using Mutagen.Bethesda.Plugins;
namespace CreationEditor;

public sealed record ReferenceCache(Dictionary<FormKey, HashSet<IFormLinkIdentifier>> Cache) {
    public static ReferenceCache operator +(ReferenceCache a, ReferenceCache b) {
        var newRefCache = new ReferenceCache(a.Cache);
        
        foreach (var (formKey, references) in b.Cache) {
            if (newRefCache.Cache.TryGetValue(formKey, out var existingReferences)) {
                foreach (var reference in references) {
                    existingReferences.Add(reference);
                }
            } else {
                newRefCache.Cache.Add(formKey, references);
            }
        }
        
        return newRefCache;
    }
}