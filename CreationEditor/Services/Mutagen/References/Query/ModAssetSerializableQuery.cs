using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class ModAssetSerializableQuery
    : IReferenceQuery<IModGetter, AssetReferenceCache<IFormLinkIdentifier>, IAssetLinkGetter, IFormLinkIdentifier>, IDisposable {
    private readonly DisposableBucket _disposableDropoff = new();
    private readonly ILogger _logger;
    private IAssetLinkCache _assetLinkCache;

    public string Name => "Mod Asset Links";

    public bool SkipResolvedAssets { get; set; } = true; // todo change back to false by default when inferred assets bug is fixed
    private const AssetLinkQuery NotResolvedQueryFlags = AssetLinkQuery.Listed | AssetLinkQuery.Inferred;

    public ModAssetSerializableQuery(
        ILogger logger,
        ILinkCacheProvider linkCacheProvider) {
        _logger = logger;
        _assetLinkCache = linkCacheProvider.LinkCache.CreateImmutableAssetLinkCache();

        linkCacheProvider.LinkCacheChanged
            .Subscribe(linkCache => {
                _assetLinkCache.Dispose();
                _assetLinkCache = linkCache.CreateImmutableAssetLinkCache();
            })
            .DisposeWith(_disposableDropoff);
    }

    public string GetSourceName(IModGetter source) => source.ModKey.FileName;

    public IModGetter? ReferenceToSource(IFormLinkIdentifier reference) => null;

    public void FillCache(IModGetter source, AssetReferenceCache<IFormLinkIdentifier> cache) {
        if (SkipResolvedAssets) {
            foreach (var record in source.EnumerateMajorRecords()) {
                try {
                    var recordLink = record.ToLinkFromRuntimeType();
                    foreach (var link in record.EnumerateAssetLinks(NotResolvedQueryFlags).Where(l => !l.IsNull)) {
                        cache.Add(link, recordLink);
                    }
                } catch (Exception e) {
                    _logger.Here().Error(e, "Error parsing asset references of {Record}", record);
                }

            }
        } else {
            foreach (var record in source.EnumerateMajorRecords()) {
                try {
                    var recordLink = record.ToLinkFromRuntimeType();
                    foreach (var assetLinkGetter in record.EnumerateAllAssetLinks(_assetLinkCache).Where(l => !l.IsNull)) {
                        cache.Add(assetLinkGetter, recordLink);

                    }
                } catch (Exception e) {
                    _logger.Here().Error(e, "Error parsing asset references of {Record}", record);
                }
            }
        }
    }

    public IEnumerable<IAssetLinkGetter> ParseRecord(IMajorRecordGetter record) {
        if (SkipResolvedAssets) {
            return record.EnumerateAssetLinks(NotResolvedQueryFlags, _assetLinkCache)
                .Where(l => !l.IsNull);
        }

        return record.EnumerateAllAssetLinks(_assetLinkCache)
            .Where(l => !l.IsNull);
    }

    public void Dispose() {
        _disposableDropoff.Dispose();
        _assetLinkCache.Dispose();
    }
}
