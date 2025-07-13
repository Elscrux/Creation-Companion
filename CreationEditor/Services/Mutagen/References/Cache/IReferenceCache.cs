namespace CreationEditor.Services.Mutagen.References.Cache;

public interface IReferenceCache<TSelf, TLink, TReference> where TSelf : IReferenceCache<TSelf, TLink, TReference> {
    static abstract TSelf CreateNew();
    void Add(TSelf otherCache);
    void Add(TLink link, TReference reference);
    void Remove(IReadOnlyList<TReference> referencesToRemove);
}
