namespace CreationEditor.Services.Cache.Validation;

public struct CacheValidationResult<TContent> {
    public bool CacheFullyInvalidated { get; }
    public IList<TContent> InvalidatedContent { get; }

    private CacheValidationResult(bool cacheFullyInvalidated, IList<TContent> invalidatedContent) {
        CacheFullyInvalidated = cacheFullyInvalidated;
        InvalidatedContent = invalidatedContent;
    }

    public static CacheValidationResult<TContent> FullyInvalid() {
        return new CacheValidationResult<TContent>(true, Array.Empty<TContent>());
    }

    public static CacheValidationResult<TContent> PartlyInvalid(IList<TContent> invalidatedContent) {
        return new CacheValidationResult<TContent>(false, invalidatedContent);
    }

    public static CacheValidationResult<TContent> Valid() {
        return new CacheValidationResult<TContent>(false, Array.Empty<TContent>());
    }
}
