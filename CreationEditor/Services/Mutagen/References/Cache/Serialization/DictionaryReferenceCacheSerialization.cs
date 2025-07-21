using System.IO.Abstractions;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Mutagen.References.Query;
using ICSharpCode.SharpZipLib.GZip;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public sealed class DictionaryReferenceCacheSerialization<TSource, TCache, TLink, TReference>(
    Func<IReadOnlyList<string>, ICacheLocationProvider> assetReferenceLocationProviderFactory,
    IReferenceCacheSerializationConfigLink<TSource, TLink, TReference> referenceCacheSerializationConfig,
    ILogger logger,
    IFileSystem fileSystem,
    params IReadOnlyList<string> path)
    : IReferenceCacheSerialization<TSource, TCache, TLink, TReference>
    where TSource : notnull
    where TCache : IDictionaryReferenceCache<TCache, TLink, TReference>
    where TLink : notnull
    where TReference : notnull {

    private readonly ICacheLocationProvider _cacheLocationProvider = assetReferenceLocationProviderFactory(path);
    private readonly Version _version = new(2, 0);

    private string GetCacheFile(TSource source, IReferenceQuery<TSource, TCache, TLink, TReference> query) {
        return _cacheLocationProvider.CacheFile(query.Name, query.GetSourceName(source));
    }

    public bool Validate(TSource source, IReferenceQuery<TSource, TCache, TLink, TReference> query) {
        var cacheFile = GetCacheFile(source, query);

        // Check if cache exist
        if (!fileSystem.File.Exists(cacheFile)) return false;

        try {
            // Open cache reader
            using var fileStream = fileSystem.File.Open(cacheFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var zip = new GZipInputStream(fileStream);
            using var reader = new BinaryReader(zip);

            // Read serialization version
            var versionString = reader.ReadString();
            if (!Version.TryParse(versionString, out var version) || !_version.Equals(version)) return false;

            // Read individual cacheable version number
            versionString = reader.ReadString();
            if (!Version.TryParse(versionString, out version) || !referenceCacheSerializationConfig.CacheVersion.Equals(version)) return false;

            return referenceCacheSerializationConfig.IsCacheUpToDate(reader, source);
        } catch (Exception e) {
            logger.Here().Warning(e, "Failed to validate cache file {File}: {Exception}", cacheFile, e.Message);
            return false;
        }
    }

    public TCache Deserialize(TSource source, IReferenceQuery<TSource, TCache, TLink, TReference> query) {
        var cacheFile = GetCacheFile(source, query);

        // Read cache file
        var cache = TCache.CreateNew();
        using var fileStream = fileSystem.File.Open(cacheFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var zip = new GZipInputStream(fileStream);
        using var reader = new BinaryReader(zip);

        try {
            // Skip versions
            reader.ReadString();
            reader.ReadString();

            // Skip hash
            referenceCacheSerializationConfig.IsCacheUpToDate(reader, source);

            // Get context
            var sourceContextString = referenceCacheSerializationConfig.ReadSource(reader);

            // Build cache
            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++) {
                // Parse link
                var link = referenceCacheSerializationConfig.ReadLink(reader);
                if (link is null) continue;

                // Parse references
                var referenceCount = reader.ReadInt32();
                var references = referenceCacheSerializationConfig
                    .ReadReferences(reader, sourceContextString, referenceCount)
                    .ToHashSet();

                cache.Cache.TryAdd(link, references);
            }
        } catch (Exception e) {
            logger.Here().Warning(e, "Failed to read cache file {File}: {Exception}", cacheFile, e.Message);
        }

        return cache;
    }

    public void Serialize(
        TSource source,
        IReferenceQuery<TSource, TCache, TLink, TReference> query,
        TCache cache) {
        // Prepare file structure
        var cacheFile = GetCacheFile(source, query);
        var info = fileSystem.FileInfo.New(cacheFile);
        info.Directory?.Create();

        // Prepare file stream
        using var fileStream = fileSystem.File.OpenWrite(info.FullName);
        using var zip = new GZipOutputStream(fileStream);
        using var writer = new BinaryWriter(zip);

        // Write main serialization version
        writer.Write(_version.ToString());

        // Write individual cacheable version number
        writer.Write(referenceCacheSerializationConfig.CacheVersion.ToString());

        // Write cache check
        referenceCacheSerializationConfig.WriteCacheValidation(writer, source);

        // Write context string
        referenceCacheSerializationConfig.WriteSource(writer, source);

        // Write cache
        writer.Write(cache.Cache.Count);
        foreach (var (link, references) in cache.Cache) {
            referenceCacheSerializationConfig.WriteLink(writer, link);
            writer.Write(references.Count);
            referenceCacheSerializationConfig.WriteReferences(writer, references);
        }
    }
}
