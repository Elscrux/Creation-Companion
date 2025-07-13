using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed class RecordReferenceCacheController(IEditorEnvironment editorEnvironment)
    : IReferenceCacheController<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> {
    public void AddLink(RecordReferenceCache cache, IFormLinkIdentifier reference, IEnumerable<IFormLinkIdentifier> linksToAdd) {
        foreach (var link in linksToAdd) {
            var references = cache.Cache.GetOrAdd(link.FormKey);

            lock (references) {
                references.Add(reference);
            }
        }
    }

    public void RemoveLink(RecordReferenceCache cache, IFormLinkIdentifier reference, IEnumerable<IFormLinkIdentifier> linksToRemove) {
        foreach (var link in linksToRemove) {
            // when the record was not part of the MUTABLE MOD before we need to reevaluate all old form from the other one too  

            if (!cache.Cache.TryGetValue(link.FormKey, out var references)) continue;

            lock (references) {
                references.Remove(reference);
            }
        }
    }

    public IEnumerable<IFormLinkIdentifier> GetLinks(IEnumerable<RecordReferenceCache> caches, IFormLinkIdentifier reference) {
        return [];
    }

    public IEnumerable<IFormLinkIdentifier> GetReferences(IReadOnlyDictionary<IModGetter, RecordReferenceCache> caches, IFormLinkIdentifier link) {
        var modKeys = caches.Keys.Select(x => x.ModKey).ToArray();
        var priorityOrderedCaches = caches
            .OrderBy(x => modKeys.IndexOf(x.Key.ModKey))
            .ToArray();

        foreach (var (mod, cache) in priorityOrderedCaches) {
            if (!cache.Cache.TryGetValue(link.FormKey, out var references)) continue;

            foreach (var reference in references.ToArray()) {
                // Skip references that are overridden by another mod
                var containingMod = priorityOrderedCaches.FirstOrDefault(x => x.Value.FormKeys.Contains(reference.FormKey));
                if (containingMod.Key.ModKey != mod.ModKey) continue;

                yield return reference;
            }
        }
    }
}
