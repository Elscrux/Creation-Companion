using System.Collections.Concurrent;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed record RecordReferenceCache(ConcurrentDictionary<FormKey, HashSet<IFormLinkIdentifier>> Cache, HashSet<FormKey> FormKeys)
    : IReferenceCache<RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> {
    public static RecordReferenceCache CreateNew() {
        return new RecordReferenceCache(new ConcurrentDictionary<FormKey, HashSet<IFormLinkIdentifier>>(), []);
    }

    public void Add(RecordReferenceCache otherCache) {
        lock (FormKeys) {
            foreach (var formKey in otherCache.FormKeys) {
                FormKeys.Add(formKey);
            }
        }

        foreach (var (formKey, references) in otherCache.Cache) {
            var existingReferences = Cache.GetOrAdd(formKey);

            lock (existingReferences) {
                foreach (var reference in references.ToArray()) {
                    existingReferences.Add(reference);
                }
            }
        }
    }

    public void Add(IFormLinkIdentifier link, IFormLinkIdentifier reference) {
        var formKey = link.FormKey;
        var references = Cache.GetOrAdd(formKey);
        lock (references) {
            references.Add(reference);
        }
        lock (FormKeys) {
            FormKeys.Add(formKey);
        }
    }

    public void Remove(IReadOnlyList<IFormLinkIdentifier> referencesToRemove) {
        foreach (var reference in referencesToRemove) {
            var formKey = reference.FormKey;
            if (!Cache.TryGetValue(formKey, out var references)) continue;

            lock (references) {
                references.Remove(reference);
            }
        }
    }
}
