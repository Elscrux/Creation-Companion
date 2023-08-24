using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public static class RecordReferenceCacheMixIn {
    /// <summary>
    /// Returns references of one form key in a link cache.
    /// This is using the priority order so winning overrides are prioritized.
    /// </summary>
    /// <param name="recordReferenceCache">reference cache to search for references</param>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="linkCache">link cache to get references from</param>
    /// <returns>form links of references</returns>
    public static IEnumerable<IFormLinkIdentifier> GetReferences(this IRecordReferenceCache recordReferenceCache, FormKey formKey, ILinkCache linkCache) {
        return recordReferenceCache.GetReferences(formKey, linkCache.PriorityOrder);
    }

    /// <summary>
    /// Returns references of one form key in a game environment
    /// </summary>
    /// <param name="recordReferenceCache">reference cache to search for references</param>
    /// <param name="formKey">form key to search references for</param>
    /// <param name="environment">environment to get references from</param>
    /// <returns>form links of references</returns>
    public static IEnumerable<IFormLinkIdentifier> GetReferences(this IRecordReferenceCache recordReferenceCache, FormKey formKey, IGameEnvironment environment) {
        return recordReferenceCache.GetReferences(formKey, environment.LinkCache);
    }
}
