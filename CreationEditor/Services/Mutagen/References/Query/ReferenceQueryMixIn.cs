using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Services.Mutagen.References.Query;

public static class ReferenceQueryMixIn {
    /// <summary>
    /// Loads all references of a link cache
    /// </summary>
    /// <param name="referenceQuery">reference query to load references with</param>
    /// <param name="linkCache">link cache to load references for</param>
    public static void LoadModReferences(IReferenceQuery referenceQuery, ILinkCache linkCache) {
        referenceQuery.LoadModReferences(linkCache.PriorityOrder);
    }

    /// <summary>
    /// Loads all references of an environment
    /// </summary>
    /// <param name="referenceQuery">reference query to load references with</param>
    /// <param name="environment">environment to load references for</param>
    public static void LoadModReferences(IReferenceQuery referenceQuery, IGameEnvironment environment) {
        LoadModReferences(referenceQuery, environment.LinkCache);
    }
}
