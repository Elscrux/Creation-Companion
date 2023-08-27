using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public sealed class AssetReferenceCacheBuilder {
    private readonly ILogger _logger;

    public AssetReferenceCacheBuilder(ILogger logger) {
        _logger = logger;
    }

    /// <summary>
    /// Builds an asset reference cache for the given asset query and source.
    /// </summary>
    /// <param name="query">Query to to parse the source with</param>
    /// <param name="source">Source to parse</param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TReference"></typeparam>
    /// <returns>Task to build the asset cache</returns>
    public async Task<AssetReferenceCache<TSource, TReference>> BuildCache<TSource, TReference>(IAssetReferenceQuery<TSource, TReference> query, TSource source)
        where TSource : notnull
        where TReference : notnull {
        if (query.AssetCaches.TryGetValue(source, out var refCache)) return refCache;

        var sourceName = query.GetName(source);
        _logger.Here().Debug("Starting to load {QueryName} Asset References of {Source}", query.QueryName, sourceName);

        AssetReferenceCache<TSource, TReference> cache;
        if (query is IAssetReferenceCacheableQuery<TSource, TReference> cacheable) {
            // Parse cache
            cache = await TryParseCache(cacheable);
        } else {
            // Parse assets
            var parseDictionary = new Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>>();
            ParseSource(source, parseDictionary);
            cache = new AssetReferenceCache<TSource, TReference>(source, parseDictionary);
        }

        _logger.Here().Debug("Finished loading {QueryName} Asset References of {Source}", query.QueryName, sourceName);

        query.AssetCaches.Add(source, cache);
        return cache;

        void ParseSource(TSource s, IDictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>> dict) {
            foreach (var result in query.ParseAssets(s)) {
                var typeCache = dict.GetOrAdd(result.AssetLink.Type, CreateNewEntry);
                var references = typeCache.GetOrAdd(result.AssetLink);
                references.Add(result.Reference);
            }
        }

        Dictionary<IAssetLinkGetter, HashSet<TReference>> CreateNewEntry() => new(AssetLinkEqualityComparer.Instance);

        async Task<AssetReferenceCache<TSource, TReference>> TryParseCache(IAssetReferenceCacheableQuery<TSource, TReference> assetReferenceCacheableQuery) {
            if (assetReferenceCacheableQuery.Serialization.Validate(source, assetReferenceCacheableQuery)) {
                // Cache is valid, deserialize it
                return await ParseCache(assetReferenceCacheableQuery);
            }

            // Cache is invalid, parse assets and serialize them
            if (cacheable is IAssetReferenceCacheableValidatableQuery<TSource, TReference> validatableQuery) {
                // Build validation cache
                await validatableQuery.CacheValidation.GetInvalidatedContent(source);
            }

            return FullyParseCache(assetReferenceCacheableQuery);
        }

        AssetReferenceCache<TSource, TReference> FullyParseCache(IAssetReferenceCacheableQuery<TSource, TReference> assetReferenceCacheableQuery) {
            var parseDictionary = new Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>>();
            ParseSource(source, parseDictionary);
            cache = new AssetReferenceCache<TSource, TReference>(source, parseDictionary);
            assetReferenceCacheableQuery.Serialization.Serialize(source, assetReferenceCacheableQuery, cache);
            return cache;
        }

        async Task<AssetReferenceCache<TSource, TReference>> ParseCache(IAssetReferenceCacheableQuery<TSource, TReference> assetReferenceCacheableQuery) {
            if (assetReferenceCacheableQuery is not IAssetReferenceCacheableValidatableQuery<TSource, TReference> { CacheValidation: {} cacheValidation }) {
                // No internal cache validation, just deserialize
                return await Task.Run(() => assetReferenceCacheableQuery.Serialization.Deserialize(source, assetReferenceCacheableQuery));
            }

            // Validate cache and parse invalidated sources
            return await ParseValidatableCache(assetReferenceCacheableQuery, cacheValidation);
        }

        async Task<AssetReferenceCache<TSource, TReference>> ParseValidatableCache(IAssetReferenceCacheableQuery<TSource, TReference> assetReferenceCacheableQuery, IInternalCacheValidation<TSource, TReference> cacheValidation) {
            // Internal cache validation, parse invalidated assets and merge with deserialized cache
            var validationResult = await cacheValidation.GetInvalidatedContent(source);

            // If fully invalidated, parse from the ground up
            if (validationResult.CacheFullyInvalidated) return FullyParseCache(cacheable);

            // Otherwise, deserialize existing cache
            var deserializationTask = Task.Run(() => assetReferenceCacheableQuery.Serialization.Deserialize(source, assetReferenceCacheableQuery));

            // If no assets were invalidated, use deserialized cache
            if (validationResult.InvalidatedContent.Count == 0) return await deserializationTask;

            // Otherwise, parse invalidated assets and merge with deserialized cache
            return await MergeCacheAndParsed(validationResult, deserializationTask, assetReferenceCacheableQuery);
        }

        async Task<AssetReferenceCache<TSource, TReference>> MergeCacheAndParsed(
            CacheValidationResult<TReference> validationResult,
            Task<AssetReferenceCache<TSource, TReference>> deserializationTask,
            IAssetReferenceCacheableQuery<TSource, TReference> assetReferenceCacheableQuery) {

            var newParseCache = new Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>>();
            var parseTask = Task.Run(() => {
                foreach (var invalidatedContent in validationResult.InvalidatedContent) {
                    if (invalidatedContent is TSource sourceContent) {
                        ParseSource(sourceContent, newParseCache);
                    }
                }
            });

            await Task.WhenAll(deserializationTask, parseTask);
            var deserializedCache = deserializationTask.Result;

            // Remove all old references from invalidated assets
            foreach (var assetReferences in deserializedCache.Cache.Values) {
                foreach (var (assetLink, references) in assetReferences) {
                    references.ExceptWith(validationResult.InvalidatedContent);
                    if (references.Count == 0) {
                        assetReferences.Remove(assetLink);
                    }
                }
            }

            // Add new references of invalidated assets
            foreach (var (assetType, assetReferences) in newParseCache) {
                var typeCache = deserializedCache.Cache.GetOrAdd(assetType, CreateNewEntry);
                foreach (var (assetLink, references) in assetReferences) {
                    var typeReferences = typeCache.GetOrAdd(assetLink);
                    typeReferences.UnionWith(references);
                }
            }

            // Serialize merged cache
            assetReferenceCacheableQuery.Serialization.Serialize(source, assetReferenceCacheableQuery, deserializedCache);
            return deserializedCache;
        }
    }
}
