namespace CreationEditor.Services.Cache;

public interface ICacheLocationProvider {
    string CacheFile(params string[] identifiers);
}
