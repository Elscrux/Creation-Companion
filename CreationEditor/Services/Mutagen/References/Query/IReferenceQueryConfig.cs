namespace CreationEditor.Services.Mutagen.References.Query;

public interface IReferenceQueryConfig<TSource, TSourceElement, TCache, TLink>
    where TSource : notnull
    where TSourceElement : notnull
    where TCache : notnull
    where TLink : notnull {
    bool CanGetLinksFromDeletedElement { get; }
    string Name { get; }
    Task<TCache> BuildCache(TSource source);
    IEnumerable<TLink> GetLinks(TSourceElement element);
}
