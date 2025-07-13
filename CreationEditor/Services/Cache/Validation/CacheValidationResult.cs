namespace CreationEditor.Services.Cache.Validation;

public struct CacheValidationResult<TContent> {
    public bool CacheFullyInvalidated { get; }
    public IReadOnlyList<TContent> InvalidatedContent { get; }
    /// <summary>
    /// Receiver is responsible for calling this action to update the cache
    /// </summary>
    public Action UpdateCache { get; set; }

    private CacheValidationResult(bool cacheFullyInvalidated, IReadOnlyList<TContent> invalidatedContent, Action updateCache) {
        CacheFullyInvalidated = cacheFullyInvalidated;
        InvalidatedContent = invalidatedContent;
        UpdateCache = updateCache;
    }

    public static CacheValidationResult<TContent> FullyInvalid(Action updateCache) {
        return new CacheValidationResult<TContent>(true, [], updateCache);
    }

    public static CacheValidationResult<TContent> PartlyInvalid(IReadOnlyList<TContent> invalidatedContent, Action updateCache) {
        return new CacheValidationResult<TContent>(false, invalidatedContent, updateCache);
    }

    public static CacheValidationResult<TContent> Valid() {
        return new CacheValidationResult<TContent>(false, [], () => {});
    }
}
