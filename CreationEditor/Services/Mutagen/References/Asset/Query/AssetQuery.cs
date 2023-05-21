using System.IO.Abstractions;
using Autofac;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Notification;
using ICSharpCode.SharpZipLib.GZip;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public abstract class AssetQuery : IDisposableDropoff {
    protected static readonly Version CacheVersion = new(1, 0, 0, 0);
    protected static readonly string[] AssetsDirectory = { "References", "Assets" };

    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    public virtual void Dispose() => _disposables.Dispose();
    public void Add(IDisposable disposable) => _disposables.Add(disposable);
}

public abstract class AssetQuery<TOrigin, TReference> : AssetQuery
    where TOrigin : notnull
    where TReference : notnull {

    public sealed record AssetReferenceCache(Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>> Cache);

    protected readonly ILogger Logger;
    protected readonly IFileSystem FileSystem;
    protected readonly ICacheLocationProvider CacheLocationProvider;
    protected readonly INotificationService NotificationService;
    protected readonly IAssetTypeService AssetTypeService;

    protected abstract bool CacheAssets { get; }

    protected abstract string QueryName { get; }

    public abstract IEnumerable<AssetQueryResult<TReference>> ParseAssets(TOrigin directory);

    protected virtual string GetName(TOrigin origin) => origin.ToString() ?? string.Empty;

    protected abstract void WriteCacheCheck(BinaryWriter writer, TOrigin origin);
    protected abstract void WriteContext(BinaryWriter writer, TOrigin origin);
    protected abstract void WriteUsages(BinaryWriter writer, IEnumerable<TReference> usages);

    protected abstract bool IsCacheUpToDate(BinaryReader reader, TOrigin origin);
    protected abstract string ReadContextString(BinaryReader reader);
    protected abstract IEnumerable<TReference> ReadUsages(BinaryReader reader, string context, int count);

    protected AssetQuery(
        ILifetimeScope lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);

        FileSystem = newScope.Resolve<IFileSystem>();
        Logger = newScope.Resolve<ILogger>();
        AssetTypeService = newScope.Resolve<IAssetTypeService>();
        CacheLocationProvider = newScope.Resolve<ICacheLocationProvider>(TypedParameter.From(AssetsDirectory.Append(QueryName).ToArray()));
        NotificationService = newScope.Resolve<INotificationService>();
    }

    public IReadOnlyDictionary<TOrigin, AssetReferenceCache> AssetCaches => _assetCaches;
    private readonly Dictionary<TOrigin, AssetReferenceCache> _assetCaches = new();

    public AssetReferenceCache? LoadAssets(TOrigin origin) {
        if (_assetCaches.ContainsKey(origin)) return null;

        Logger.Here().Debug("Starting to load {QueryName} Asset References of {Origin}", QueryName, GetName(origin));
        var assetReferenceCache = CacheValid(origin) ? LoadAssetCache(origin) : BuildAssetCache(origin);
        _assetCaches.Add(origin, assetReferenceCache);
        Logger.Here().Debug("Finished loading {QueryName} Asset References of {Origin}", QueryName, GetName(origin));

        return assetReferenceCache;
    }

    private bool CacheValid(TOrigin origin) {
        if (!CacheAssets) return false;

        var cacheFile = CacheLocationProvider.CacheFile(GetName(origin));

        // Check if cache exist
        if (!FileSystem.File.Exists(cacheFile)) return false;

        // Open cache reader
        using var fileStream = FileSystem.File.OpenRead(cacheFile);
        using var zip = new GZipInputStream(fileStream);
        using var reader = new BinaryReader(zip);

        // Read version in cache
        if (!Version.TryParse(reader.ReadString(), out var version)
         || !version.Equals(CacheVersion)) return false;

        return IsCacheUpToDate(reader, origin);
    }

    protected AssetReferenceCache BuildAssetCache(TOrigin origin) {
        var cache = new Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>>();

        // Parse assets
        foreach (var result in ParseAssets(origin)) {
            var typeCache = cache.GetOrAdd(result.AssetLink.Type, () => new Dictionary<IAssetLinkGetter, HashSet<TReference>>(AssetLinkEqualityComparer.Instance));
            var assets = typeCache.GetOrAdd(result.AssetLink);
            assets.Add(result.Reference);
        }

        // Write assets to cache file
        if (!CacheAssets) return new AssetReferenceCache(cache);

        // Prepare file structure
        var info = FileSystem.FileInfo.New(CacheLocationProvider.CacheFile(GetName(origin)));
        info.Directory?.Create();

        // Prepare file stream
        using var fileStream = FileSystem.File.OpenWrite(info.FullName);
        using var zip = new GZipOutputStream(fileStream);
        using var writer = new BinaryWriter(zip);

        // Write version
        writer.Write(CacheVersion.ToString());

        // Write cache check
        WriteCacheCheck(writer, origin);

        // Write context string
        WriteContext(writer, origin);

        // Write cache
        writer.Write(cache.Count);
        using var countingNotifier = new CountingNotifier(NotificationService, "Saving Cache", cache.Count);
        foreach (var (assetType, assets) in cache) {
            countingNotifier.NextStep();

            writer.Write(AssetTypeService.GetAssetTypeIdentifier(assetType));
            writer.Write(assets.Count);
            foreach (var (name, usages) in assets) {
                writer.Write(name.DataRelativePath);
                writer.Write(usages.Count);
                WriteUsages(writer, usages);
            }
        }

        return new AssetReferenceCache(cache);
    }

    protected AssetReferenceCache LoadAssetCache(TOrigin origin) {
        var cache = new Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>>();
        var cacheFile = CacheLocationProvider.CacheFile(GetName(origin));

        if (!FileSystem.File.Exists(cacheFile)) return new AssetReferenceCache(cache);

        // Read cache file
        var fileStream = FileSystem.File.OpenRead(cacheFile);
        var zip = new GZipInputStream(fileStream);
        using (var reader = new BinaryReader(zip)) {
            try {
                // Skip version and checksum
                reader.ReadString();
                IsCacheUpToDate(reader, origin);

                // Get context
                var contextString = ReadContextString(reader);

                // Build cache
                var count = reader.ReadInt32();
                using var counter = new CountingNotifier(NotificationService, "Reading Cache", count);
                for (var i = 0; i < count; i++) {
                    counter.NextStep();

                    var assetTypeString = reader.ReadString();
                    var assetType = AssetTypeService.GetAssetTypeFromIdentifier(assetTypeString);

                    // Parse assets
                    var assets = new Dictionary<IAssetLinkGetter, HashSet<TReference>>(AssetLinkEqualityComparer.Instance);
                    var assetCount = reader.ReadInt32();
                    for (var j = 0; j < assetCount; j++) {
                        // Parse asset link
                        var assetPath = reader.ReadString();
                        var assetLink = AssetTypeService.GetAssetLink(assetPath, assetType);

                        var assetUsageCount = reader.ReadInt32();
                        var usages = ReadUsages(reader, contextString, assetUsageCount);

                        assets.Add(assetLink, new HashSet<TReference>(usages));
                    }

                    cache.Add(assetType, assets);
                }
            } catch (Exception e) {
                Logger.Here().Warning("Failed to read cache file {File}: {Exception}", cacheFile, e.Message);
            }
        }

        return new AssetReferenceCache(cache);
    }
}
