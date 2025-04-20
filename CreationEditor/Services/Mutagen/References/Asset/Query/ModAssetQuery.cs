using System.Collections.Concurrent;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ModAssetQuery : IAssetReferenceCacheableQuery<IModGetter, IFormLinkIdentifier>, IDisposable {
    private readonly DisposableBucket _disposableDropoff = new();

    private readonly ILogger _logger;
    private readonly IDataSourceService _dataSourceService;
    private readonly IMutagenTypeProvider _mutagenTypeProvider;
    private IAssetLinkCache _assetLinkCache;

    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<IModGetter, IFormLinkIdentifier> Serialization { get; }
    public string QueryName => "Mod";
    public IDictionary<IModGetter, AssetReferenceCache<IModGetter, IFormLinkIdentifier>> AssetCaches { get; }
        = new ConcurrentDictionary<IModGetter, AssetReferenceCache<IModGetter, IFormLinkIdentifier>>();

    public bool SkipResolvedAssets { get; set; } = true; // todo change back to false by default when inferred assets bug is fixed

    public ModAssetQuery(
        ILogger logger,
        IDataSourceService dataSourceService,
        ILinkCacheProvider linkCacheProvider,
        IMutagenTypeProvider mutagenTypeProvider,
        IAssetReferenceSerialization<IModGetter, IFormLinkIdentifier> serialization) {
        _logger = logger;
        _dataSourceService = dataSourceService;
        _mutagenTypeProvider = mutagenTypeProvider;
        _assetLinkCache = linkCacheProvider.LinkCache.CreateImmutableAssetLinkCache();
        Serialization = serialization;

        linkCacheProvider.LinkCacheChanged
            .Subscribe(linkCache => {
                _assetLinkCache.Dispose();
                _assetLinkCache = linkCache.CreateImmutableAssetLinkCache();
            })
            .DisposeWith(_disposableDropoff);
    }

    public string GetName(IModGetter source) => source.ModKey.FileName;
    public IModGetter? ReferenceToSource(IFormLinkIdentifier reference) => null;
    public IEnumerable<AssetQueryResult<IFormLinkIdentifier>> ParseAssets(IModGetter source) {
        if (SkipResolvedAssets) {
            foreach (var record in source.EnumerateMajorRecords()) {
                IEnumerable<AssetQueryResult<IFormLinkIdentifier>> results;
                try {
                    results = record.EnumerateAssetLinks(AssetLinkQuery.Listed | AssetLinkQuery.Inferred)
                        .Where(l => !l.IsNull)
                        .Select(l => new AssetQueryResult<IFormLinkIdentifier>(l, record.ToLinkFromRuntimeType()))
                        .ToArray();
                } catch (Exception e) {
                    _logger.Here().Error(e, "Error parsing asset references of {Record}", record);
                    results = [];
                }

                foreach (var result in results) {
                    yield return result;
                }
            }
        } else {
            foreach (var record in source.EnumerateMajorRecords()) {
                IEnumerable<AssetQueryResult<IFormLinkIdentifier>> results;
                try {
                    results = record.EnumerateAllAssetLinks(_assetLinkCache)
                        .Where(l => !l.IsNull)
                        .Select(l => new AssetQueryResult<IFormLinkIdentifier>(l, record.ToLinkFromRuntimeType()))
                        .ToArray();
                } catch (Exception e) {
                    _logger.Here().Error(e, "Error parsing asset references of {Record}", record);
                    results = [];
                }

                foreach (var result in results) {
                    yield return result;
                }
            }
        }
    }

    public void WriteCacheValidation(BinaryWriter writer, IModGetter mod) {
        if (!_dataSourceService.TryGetFileLink(mod.ModKey.FileName.String, out var modLink)) return;
        if (!modLink.Exists()) return;

        var hash = modLink.FileSystem.GetFileHash(modLink.FullPath);
        writer.Write(hash);
    }

    public void WriteContext(BinaryWriter writer, IModGetter source) {
        // Write game
        writer.Write(_mutagenTypeProvider.GetGameName(source));
    }

    public void WriteReferences(BinaryWriter writer, IEnumerable<IFormLinkIdentifier> references) {
        foreach (var usage in references) {
            writer.Write(usage.FormKey.ToString());
            writer.Write(_mutagenTypeProvider.GetTypeName(usage));
        }
    }

    public bool IsCacheUpToDate(BinaryReader reader, IModGetter source) {
        if (!_dataSourceService.TryGetFileLink(source.ModKey.FileName.String, out var modLink)) return false;
        if (!modLink.Exists()) return false;

        // Read hash in cache
        var hash = reader.ReadBytes(modLink.FileSystem.GetHashBytesLength());

        // Validate hash
        return modLink.FileSystem.IsFileHashValid(modLink.FullPath, hash);
    }

    public string ReadContextString(BinaryReader reader) {
        // Read game string
        var game = reader.ReadString();
        return game;
    }

    public IEnumerable<IFormLinkIdentifier> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount) {
        for (var i = 0; i < assetReferenceCount; i++) {
            var referenceFormKey = FormKey.Factory(reader.ReadString());
            var typeString = reader.ReadString();
            var type = _mutagenTypeProvider.GetType(contextString, typeString);

            yield return new FormLinkInformation(referenceFormKey, type);
        }
    }

    public void Dispose() {
        _disposableDropoff.Dispose();
        _assetLinkCache.Dispose();
    }
}
