using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed class AssetDictionaryReferenceCacheController<TLink>
    : IReferenceCacheController<IDataSource, AssetDictionaryReferenceCache<TLink>, TLink, DataRelativePath>
    where TLink : notnull {
    public void AddLink(
        AssetDictionaryReferenceCache<TLink> cache,
        DataRelativePath reference,
        IEnumerable<TLink> linksToAdd) {
        foreach (var link in linksToAdd) {
            var references = cache.Cache.GetOrAdd(link, _ => []);
            lock (references) {
                references.Add(reference);
            }
        }
    }

    public void RemoveLink(
        AssetDictionaryReferenceCache<TLink> cache,
        DataRelativePath reference,
        IEnumerable<TLink> linksToRemove) {
        foreach (var link in linksToRemove) {
            if (!cache.Cache.TryGetValue(link, out var references)) continue;

            lock (references) {
                references.Remove(reference);
            }
        }
    }

    public IEnumerable<TLink> GetLinks(
        IEnumerable<AssetDictionaryReferenceCache<TLink>> caches,
        DataRelativePath reference) {
        foreach (var cache in caches) {
            foreach (var (editorId, references) in cache.Cache) {
                bool contains;
                lock (references) {
                    contains = references.Contains(reference);
                }

                if (!contains) continue;

                yield return editorId;
            }
        }
    }

    public IEnumerable<DataRelativePath> GetReferences(
        IReadOnlyDictionary<IDataSource, AssetDictionaryReferenceCache<TLink>> caches,
        TLink link) {
        foreach (var (_, cache) in caches) {
            if (!cache.Cache.TryGetValue(link, out var references)) continue;

            foreach (var reference in references.ToArray()) {
                yield return reference;
            }
        }
    }
}
