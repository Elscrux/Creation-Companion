using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Query;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed class ReferenceCacheBuilder(ILogger logger) {
    /// <summary>
    /// Builds a reference cache for the given query and source.
    /// </summary>
    /// <param name="source">Source to parse</param>
    /// <param name="query">Query to parse the source with</param>
    /// <param name="serialization">Serialization to use for the cache, if any</param>
    /// <param name="serializationValidation">Validation to use for the cache serialization, if any</param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TCache"></typeparam>
    /// <typeparam name="TLink"></typeparam>
    /// <typeparam name="TReference"></typeparam>
    /// <returns>Task to build the cache</returns>
    public async Task<TCache> BuildCache<TSource, TCache, TLink, TReference>(
        TSource source,
        IReferenceQuery<TSource, TCache, TLink, TReference> query,
        IReferenceCacheSerialization<TSource, TCache, TLink, TReference>? serialization = null,
        IInternalCacheValidation<TSource, TReference>? serializationValidation = null)
        where TSource : notnull
        where TCache : IReferenceCache<TCache, TLink, TReference>
        where TLink : notnull
        where TReference : notnull {
        var sourceName = query.GetSourceName(source);
        logger.Here().Debug("Starting to load {QueryName} References of {Source}", query.Name, sourceName);

        TCache cache;
        if (serialization is not null) {
            // Parse cache
            cache = await TryParseCache();
        } else {
            // Parse source
            cache = TCache.CreateNew();
            query.FillCache(source, cache);
        }

        logger.Here().Debug("Finished loading {QueryName} References of {Source}", query.Name, sourceName);

        return cache;

        async Task<TCache> TryParseCache() {
            if (serialization.Validate(source, query)) {
                // Cache is valid, deserialize it
                return await ParseCache();
            }

            // Cache is invalid, parse source and serialize them
            Task<CacheValidationResult<TReference>>? updateValidationCache = null;
            if (serializationValidation is not null) {
                // Build validation cache to use for validation next time
                updateValidationCache = serializationValidation.GetInvalidatedContent(source);
            }

            logger.Here().Debug("Invalid cache for {Source}, parsing everything", sourceName);
            var referenceCache = FullyParseCache();

            // In case of a validation cache, update it only after the cache has been fully parsed
            // This ensures that the cache is only updated if the parsing was successful
            // Otherwise, the validation cache would be updated when the reference cache is not up to date
            if (updateValidationCache is not null) {
                var cacheValidationResult = await updateValidationCache;
                cacheValidationResult.UpdateCache();
            }

            return referenceCache;
        }

        TCache FullyParseCache() {
            cache = TCache.CreateNew();
            query.FillCache(source, cache);
            serialization.Serialize(source, query, cache);
            return cache;
        }

        Task<TCache> ParseCache() {
            // Validate cache and parse invalidated sources
            if (serializationValidation is not null) return ParseValidatableCache();

            // No internal cache validation, just deserialize
            logger.Here().Debug("Valid cache, no cache content validation needed for {Source}, deserializing cache", sourceName);
            return Task.Run(() => serialization.Deserialize(source, query));
        }

        async Task<TCache> ParseValidatableCache() {
            // Internal cache validation, parse invalidated elements and merge with deserialized cache
            var validationResult = await serializationValidation.GetInvalidatedContent(source);

            // If fully invalidated, parse from the ground up
            if (validationResult.CacheFullyInvalidated) {
                logger.Here().Debug("Fully invalidated cache content for {Source}, parsing everything", sourceName);
                return FullyParseCache();
            }

            // Otherwise, deserialize existing cache
            var deserializationTask = Task.Run(() => serialization.Deserialize(source, query));

            // If no elements were invalidated, use deserialized cache
            if (validationResult.InvalidatedContent.Count == 0) {
                logger.Here().Debug("Valid cache content {Source}, deserializing cache", sourceName);
                return await deserializationTask;
            }

            // Otherwise, parse invalidated elements and merge with deserialized cache
            logger.Here().Debug(
                "Partly invalidated cache content for {Source}, parsing invalidated elements and using {ValidatedContentCount} validated elements",
                sourceName,
                validationResult.InvalidatedContent.Count);
            return await MergeCacheAndParsed(validationResult, deserializationTask);
        }

        async Task<TCache> MergeCacheAndParsed(CacheValidationResult<TReference> validationResult, Task<TCache> deserializationTask) {
            var newParsedCache = TCache.CreateNew();
            var parseTask = Task.Run(() => {
                foreach (var invalidatedContent in validationResult.InvalidatedContent) {
                    var newSource = query.ReferenceToSource(invalidatedContent);
                    if (newSource is not null) {
                        query.FillCache(newSource, newParsedCache);
                    }
                }
            });

            await Task.WhenAll(deserializationTask, parseTask);
            var deserializedCache = deserializationTask.Result;

            // Remove all old references from invalidated elements
            deserializedCache.Remove(validationResult.InvalidatedContent);

            // Add new references of invalidated elements
            deserializedCache.Add(newParsedCache);

            // Serialize merged cache
            serialization.Serialize(source, query, deserializedCache);

            // Update validation cache
            validationResult.UpdateCache();

            return deserializedCache;
        }
    }
}
