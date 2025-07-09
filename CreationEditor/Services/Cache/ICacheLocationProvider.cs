namespace CreationEditor.Services.Cache;

public interface ICacheLocationProvider {
    /// <summary>
    /// Get the cache file path for a set of identifiers
    /// </summary>
    /// <param name="identifiers">Identifiers to build a path from</param>
    /// <returns>Full path for the cache file</returns>
    string CacheFile(params IReadOnlyList<string> identifiers);
}
