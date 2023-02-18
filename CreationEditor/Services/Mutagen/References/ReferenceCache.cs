using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Services.Mutagen.References;

public sealed record ReferenceCache(Dictionary<FormKey, HashSet<IFormLinkIdentifier>> Cache) {
    public static ReferenceCache operator +(ReferenceCache a, ReferenceCache b) {
        var newRefCache = new ReferenceCache(a.Cache);

        foreach (var (formKey, references) in b.Cache) {
            var existingReferences = newRefCache.Cache.GetOrAdd(formKey, () => new HashSet<IFormLinkIdentifier>());

            foreach (var reference in references) {
                existingReferences.Add(reference);
            }
        }

        return newRefCache;
    }
}
