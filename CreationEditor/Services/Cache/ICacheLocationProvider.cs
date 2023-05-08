namespace CreationEditor.Services.Cache;

public interface ICacheLocationProvider {
    string CacheFile(params string[] identifiers);
    string TempCacheFile(params string[] identifiers);
}
