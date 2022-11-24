using Mutagen.Bethesda.Plugins;
namespace CreationEditor;

public record ReferenceCache(Dictionary<FormKey, HashSet<IFormLinkIdentifier>> Cache) {
    public static ReferenceCache operator +(ReferenceCache a, ReferenceCache b) {
        var newRefCache = new ReferenceCache(a.Cache);
        
        foreach (var (formKey, references) in b.Cache) {
            if (newRefCache.Cache.ContainsKey(formKey)) {
                foreach (var reference in references) {
                    newRefCache.Cache[formKey].Add(reference);
                }
            } else {
                newRefCache.Cache.Add(formKey, references);
            }
        }
        
        return newRefCache;
    }
}