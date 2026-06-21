using System.Collections.Concurrent;
namespace CreationEditor.Services.Mutagen.References.Cache;

public interface IDictionaryReferenceCache<TSelf, TLink, TReference> : IReferenceCache<TSelf, TLink, TReference>
    where TSelf : IReferenceCache<TSelf, TLink, TReference>
    where TLink : notnull
    where TReference : notnull {
    ConcurrentDictionary<TLink, HashSet<TReference>> Cache { get; }
    ConcurrentDictionary<TReference, HashSet<TLink>> ReverseCache { get; }
}
