namespace CreationEditor.Services.Cache.Validation;

public struct CacheValidationResult<TContent> {
    private readonly Action _updateCache;

    private int _hasUpdatedCache;

    public bool CacheFullyInvalidated { get; }
    public IReadOnlyList<TContent> InvalidatedContent { get; }

    private CacheValidationResult(bool cacheFullyInvalidated, IReadOnlyList<TContent> invalidatedContent, Action updateCache) {
        CacheFullyInvalidated = cacheFullyInvalidated;
        InvalidatedContent = invalidatedContent;
        _updateCache = updateCache;
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

    /// <summary>
    /// Receiver is responsible for calling this action to update the cache
    /// </summary>
    public void UpdateCache() {
        if (Interlocked.Exchange(ref _hasUpdatedCache, 1) == 0) return;

        _updateCache();
    } 
}
