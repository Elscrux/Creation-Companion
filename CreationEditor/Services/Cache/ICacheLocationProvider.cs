using Noggog;
namespace CreationEditor.Services.Cache;

public interface ICacheLocationProvider {
    FilePath CacheFile(string name);
    FilePath TempCacheFile(string name);
}
