using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Mutagen.References.Query;
using ICSharpCode.SharpZipLib.GZip;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public sealed class AssetReferenceCacheSerialization<TSource, TReference>(
    Func<IReadOnlyList<string>, ICacheLocationProvider> assetReferenceLocationProviderFactory,
    IReferenceCacheSerializationConfig<TSource, TReference> referenceCacheSerializationConfig,
    IAssetTypeService assetTypeService,
    ILogger logger,
    IFileSystem fileSystem)
    : IReferenceCacheSerialization<TSource, AssetReferenceCache<TReference>, IAssetLinkGetter, TReference>
    where TSource : notnull
    where TReference : notnull {

    private readonly ICacheLocationProvider _cacheLocationProvider = assetReferenceLocationProviderFactory(["References"]);
    private readonly Version _version = new(2, 0);

    private string GetCacheFile(
        TSource source,
        IReferenceQuery<TSource, AssetReferenceCache<TReference>, IAssetLinkGetter, TReference> query) {
        return _cacheLocationProvider.CacheFile(query.Name, query.GetSourceName(source));
    }

    public bool Validate(
        TSource source,
        IReferenceQuery<TSource, AssetReferenceCache<TReference>, IAssetLinkGetter, TReference> query) {
        var cacheFile = GetCacheFile(source, query);

        // Check if cache exist
        if (!fileSystem.File.Exists(cacheFile)) return false;

        try {
            // Open cache reader
            using var fileStream = fileSystem.File.OpenRead(cacheFile);
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

    public AssetReferenceCache<TReference> Deserialize(
        TSource source,
        IReferenceQuery<TSource, AssetReferenceCache<TReference>, IAssetLinkGetter, TReference> query) {
        var cacheFile = GetCacheFile(source, query);

        // Read cache file
        var cache = new ConcurrentDictionary<IAssetType, ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>>();
        using var fileStream = fileSystem.File.OpenRead(cacheFile);
        using var zip = new GZipInputStream(fileStream);
        using (var reader = new BinaryReader(zip)) {
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
                    var assetTypeString = reader.ReadString();
                    var assetType = assetTypeService.GetAssetTypeFromIdentifier(assetTypeString);

                    // Parse assets
                    var assets = new ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>(AssetLinkEqualityComparer.Instance);
                    var assetCount = reader.ReadInt32();
                    for (var j = 0; j < assetCount; j++) {
                        // Parse asset link
                        var assetPath = reader.ReadString();
                        var assetLink = assetTypeService.GetAssetLink(assetPath, assetType);
                        if (assetLink is null) continue;

                        var assetUsageCount = reader.ReadInt32();
                        var references = referenceCacheSerializationConfig
                            .ReadReferences(reader, sourceContextString, assetUsageCount)
                            .ToHashSet();

                        assets.TryAdd(assetLink, references);
                    }

                    cache.TryAdd(assetType, assets);
                }
            } catch (Exception e) {
                logger.Here().Warning(e, "Failed to read cache file {File}: {Exception}", cacheFile, e.Message);
            }
        }

        return new AssetReferenceCache<TReference>(cache);
    }

    public void Serialize(
        TSource source,
        IReferenceQuery<TSource, AssetReferenceCache<TReference>, IAssetLinkGetter, TReference> query,
        AssetReferenceCache<TReference> cache) {
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
        foreach (var (assetType, assets) in cache.Cache) {
            writer.Write(assetTypeService.GetAssetTypeIdentifier(assetType));
            writer.Write(assets.Count);
            foreach (var (name, references) in assets) {
                writer.Write(name.DataRelativePath.Path);
                writer.Write(references.Count);
                referenceCacheSerializationConfig.WriteReferences(writer, references);
            }
        }
    }
}
