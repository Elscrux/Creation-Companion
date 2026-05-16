using CreationEditor.Services.Asset;
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
    private readonly IAssetTypeService _assetTypeService;
    private IAssetLinkCache _assetLinkCache;

    public string Name => "Mod Asset Links";

    public ModAssetSerializableQuery(
        ILogger logger,
        IAssetTypeService assetTypeService,
        ILinkCacheProvider linkCacheProvider) {
        _logger = logger;
        _assetTypeService = assetTypeService;
        _assetLinkCache = linkCacheProvider.LinkCache.CreateImmutableAssetLinkCache();

        linkCacheProvider.LinkCacheChanged
            .Subscribe(linkCache => {
                _assetLinkCache.Dispose();
                _assetLinkCache = linkCache.CreateImmutableAssetLinkCache();
            })
            .DisposeWith(_disposableDropoff);
    }

    public string GetSourceName(IModGetter source) => source.ModKey.FileName;

    public void FillCache(IModGetter source, AssetReferenceCache<IFormLinkIdentifier> cache) {
        foreach (var record in source.EnumerateMajorRecords()) {
            try {
                var recordLink = record.ToLinkFromRuntimeType();
                foreach (var assetLinkGetter in record.EnumerateAllAssetLinks(_assetLinkCache).Where(l => !l.IsNull)) {
                    // TODO remove when Mutagen fixes bugs with Behavior and BodyTexture files in races (after that run this again to see if anything else doesn't match)
                    if (!assetLinkGetter.AssetTypeInstance.FileExtensions.Contains(assetLinkGetter.DataRelativePath.Extension)) {
                        var assetLink = _assetTypeService.GetAssetLink(assetLinkGetter.DataRelativePath.Path);
                        if (assetLink is not null) {
                            cache.Add(assetLink, recordLink);
                            continue;
                        }
                    }

                    cache.Add(assetLinkGetter, recordLink);
                }
            } catch (Exception e) {
                _logger.Here().Error(e, "Error parsing asset references of {Record}", record);
            }
        }
    }

    public void FillCache(IFormLinkIdentifier reference, AssetReferenceCache<IFormLinkIdentifier> cache) {}

    public IEnumerable<IAssetLinkGetter> ParseRecord(IMajorRecordGetter record) {
        return record.EnumerateAllAssetLinks(_assetLinkCache)
            .Where(l => !l.IsNull);
    }

    public void Dispose() {
        _disposableDropoff.Dispose();
        _assetLinkCache.Dispose();
    }
}
