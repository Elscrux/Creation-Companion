﻿using CreationEditor.Services.Cache.Validation;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public interface IAssetReferenceCacheableValidatableQuery<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {
    IInternalCacheValidation<TSource, TReference> CacheValidation { get; }
}
