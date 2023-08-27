namespace CreationEditor.Services.Cache.Validation;

public interface IInternalCacheValidation<TSource, TReference> {
    /// <summary>
    /// Checks if the cache is internally still valid
    /// and returns which part of the cache need to be re-evaluated.
    /// </summary>
    /// <param name="source">Main source of the cache</param>
    /// <returns>Returns a validation result with CacheFullyInvalidated true if only parts of the cache are invalid.
    /// In this case InvalidatedContent contains the invalidated contents.
    /// If CacheFullyInvalidated is false if the whole cache has to be re-evaluated</returns>
    Task<CacheValidationResult<TReference>> GetInvalidatedContent(TSource source);
}
