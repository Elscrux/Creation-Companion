using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using CreationEditor.Services.Notification;
using ICSharpCode.SharpZipLib.GZip;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;

public sealed class BinaryAssetReferenceSerialization<TSource, TReference>(
    Func<string[], ICacheLocationProvider> assetReferenceLocationProviderFactory,
    IAssetTypeService assetTypeService,
    INotificationService notificationService,
    ILogger logger,
    IFileSystem fileSystem)
    : IAssetReferenceSerialization<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {

    private readonly ICacheLocationProvider _cacheLocationProvider = assetReferenceLocationProviderFactory(["References", "Assets"]);
    private readonly Version _version = new(2, 0);

    private string GetCacheFile(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery) {
        return _cacheLocationProvider.CacheFile(cacheableQuery.QueryName, cacheableQuery.GetName(source));
    }

    public bool Validate(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery) {
        var cacheFile = GetCacheFile(source, cacheableQuery);

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
            if (!Version.TryParse(versionString, out version) || !cacheableQuery.CacheVersion.Equals(version)) return false;

            return cacheableQuery.IsCacheUpToDate(reader, source);
        } catch (Exception e) {
            logger.Here().Warning(e, "Failed to validate cache file {File}: {Exception}", cacheFile, e.Message);
            return false;
        }
    }

    public AssetReferenceCache<TSource, TReference> Deserialize(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery) {
        var cacheFile = GetCacheFile(source, cacheableQuery);

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
                cacheableQuery.IsCacheUpToDate(reader, source);

                // Get context
                var contextString = cacheableQuery.ReadContextString(reader);

                // Build cache
                var count = reader.ReadInt32();
                using var counter = new CountingNotifier(notificationService, "Reading Cache", count);
                for (var i = 0; i < count; i++) {
                    counter.NextStep();

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
                        var usages = cacheableQuery
                            .ReadReferences(reader, contextString, assetUsageCount)
                            .ToHashSet();

                        assets.TryAdd(assetLink, usages);
                    }

                    cache.TryAdd(assetType, assets);
                }
            } catch (Exception e) {
                logger.Here().Warning(e, "Failed to read cache file {File}: {Exception}", cacheFile, e.Message);
            }
        }

        return new AssetReferenceCache<TSource, TReference>(source, cache);
    }

    public void Serialize(
        TSource source,
        IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery,
        AssetReferenceCache<TSource, TReference> cache) {
        // Prepare file structure
        var cacheFile = GetCacheFile(source, cacheableQuery);
        var info = fileSystem.FileInfo.New(cacheFile);
        info.Directory?.Create();

        // Prepare file stream
        using var fileStream = fileSystem.File.OpenWrite(info.FullName);
        using var zip = new GZipOutputStream(fileStream);
        using var writer = new BinaryWriter(zip);

        // Write main serialization version
        writer.Write(_version.ToString());

        // Write individual cacheable version number
        writer.Write(cacheableQuery.CacheVersion.ToString());

        // Write cache check
        cacheableQuery.WriteCacheValidation(writer, source);

        // Write context string
        cacheableQuery.WriteContext(writer, source);

        // Write cache
        writer.Write(cache.Cache.Count);
        using var countingNotifier = new CountingNotifier(notificationService, "Saving Cache", cache.Cache.Count);
        foreach (var (assetType, assets) in cache.Cache) {
            countingNotifier.NextStep();

            writer.Write(assetTypeService.GetAssetTypeIdentifier(assetType));
            writer.Write(assets.Count);
            foreach (var (name, usages) in assets) {
                writer.Write(name.DataRelativePath.Path);
                writer.Write(usages.Count);
                cacheableQuery.WriteReferences(writer, usages);
            }
        }
    }
}

public interface IBinaryAssetReferenceSerializationStrategy {
    AssetReferenceCache<TSource, TReference> Deserialize<TSource, TReference>(
        IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery,
        BinaryReader reader)
        where TReference : notnull
        where TSource : notnull;

    void Serialize<TSource, TReference>(
        AssetReferenceCache<TSource, TReference> cache,
        IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery,
        BinaryWriter writer)
        where TReference : notnull
        where TSource : notnull;
}
//
// public class DefaultBinaryAssetReferenceSerializationStrategy : IBinaryAssetReferenceSerializationStrategy {
//     private readonly INotificationService _notificationService;
//     private readonly IAssetTypeService _assetTypeService;
//
//     public DefaultBinaryAssetReferenceSerializationStrategy(
//         INotificationService notificationService,
//         IAssetTypeService assetTypeService) {
//         _notificationService = notificationService;
//         _assetTypeService = assetTypeService;
//     }
//
//     public AssetReferenceCache<TSource, TReference> Deserialize<TSource, TReference>(IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery, string contextString, BinaryReader reader)
//         where TSource : notnull
//         where TReference : notnull {
//         var count = reader.ReadInt32();
//         using var counter = new CountingNotifier(_notificationService, "Reading Cache", count);
//         for (var i = 0; i < count; i++) {
//             counter.NextStep();
//
//             var assetTypeString = reader.ReadString();
//             var assetType = _assetTypeService.GetAssetTypeFromIdentifier(assetTypeString);
//
//             // Parse assets
//             var assets = new Dictionary<IAssetLinkGetter, HashSet<TReference>>(AssetLinkEqualityComparer.Instance);
//             var assetCount = reader.ReadInt32();
//             for (var j = 0; j < assetCount; j++) {
//                 // Parse asset link
//                 var assetPath = reader.ReadString();
//                 var assetLink = _assetTypeService.GetAssetLink(assetPath, assetType);
//
//                 var assetUsageCount = reader.ReadInt32();
//                 var usages = cacheableQuery.ReadUsages(reader, contextString, assetUsageCount);
//
//                 assets.Add(assetLink, new HashSet<TReference>(usages));
//             }
//
//             cache.Add(assetType, assets);
//         }
//     }
//
//     public void Serialize<TSource, TReference>(AssetReferenceCache<TSource, TReference> cache, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery, BinaryWriter writer)
//         where TSource : notnull
//         where TReference : notnull {
//         writer.Write(cache.Cache.Count);
//         using var countingNotifier = new CountingNotifier(_notificationService, "Saving Cache", cache.Cache.Count);
//         foreach (var (assetType, assets) in cache.Cache) {
//             countingNotifier.NextStep();
//
//             writer.Write(_assetTypeService.GetAssetTypeIdentifier(assetType));
//             writer.Write(assets.Count);
//             foreach (var (name, usages) in assets) {
//                 writer.Write(name.DataRelativePath);
//                 writer.Write(usages.Count);
//                 cacheableQuery.WriteUsages(writer, usages);
//             }
//         }
//     }
// }
//
// public class ReversedBinaryAssetReferenceSerializationStrategy : IBinaryAssetReferenceSerializationStrategy {
//     public AssetReferenceCache<TSource, TReference> Deserialize<TSource, TReference>(IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery, BinaryReader reader)
//         where TSource : notnull
//         where TReference : notnull {
//         
//     }
//
//     public void Serialize<TSource, TReference>(AssetReferenceCache<TSource, TReference> cache, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery, BinaryWriter writer)
//         where TSource : notnull
//         where TReference : notnull {
//         
//     }
// }
